using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;
using System.Diagnostics;
using System.Buffers;

namespace SyslogSharp.Networking;

/// <summary>
/// Provides an abstract base class for receiving and processing UDP packets asynchronously.
/// </summary>
/// <remarks>This class is designed to handle UDP packet reception and parsing. It provides a mechanism to bind to
/// a specific endpoint, receive data asynchronously, and raise an event when a valid packet is parsed. Derived classes
/// must implement the <see cref="TryParsePacket"/> method to define how incoming packets are parsed into the specified
/// type <typeparamref name="T"/>.</remarks>
/// <typeparam name="T">The type of data parsed from received UDP packets.</typeparam>
internal abstract class UdpReceiver<T> : IDisposable
{
    private readonly ArrayPool<byte> _bufferPool = ArrayPool<byte>.Shared;
    private Socket _socket;
    private bool _disposed;
    private CancellationTokenSource? _cts;
    private SocketAsyncEventArgs? _receiveEventArgs;
    private readonly Channel<Packet> _channel;
    private readonly Stopwatch _stopWatch;
    private long _messagesReceived;
    private long _bytesReceived;

    /// <summary>
    /// Gets the endpoint on which the server is configured to listen for incoming connections.
    /// </summary>
    public EndPoint ListenEndpoint { get; }

    /// <summary>
    /// Gets the protocol type used by the listener to accept incoming connections.
    /// </summary>
    public System.Net.Sockets.ProtocolType ListenProtocol { get; }

    /// <summary>
    /// Gets the number of bytes of data that are available to be read from the underlying socket.
    /// </summary>
    public int AvailableBytes => _socket?.Available ?? 0;

    /// <summary>
    /// Occurs when a message or data of type <typeparamref name="T"/> is received.
    /// </summary>
    /// <remarks>Subscribers can handle this event to process the received data. The event provides an
    /// instance of <see cref="EventArgs"/> containing the received data of type <typeparamref name="T"/>.</remarks>
    public event EventHandler<T>? Received;

    /// <summary>
    /// Gets or sets the logger used to log messages for the UDP server.
    /// </summary>
    public ILogger<UdpServer> Logger { get; set; } = NullLogger<UdpServer>.Instance;

    /// <summary>
    /// Initializes a new instance of the <see cref="UdpReceiver"/> class with the specified listening endpoint.
    /// </summary>
    /// <remarks>This constructor sets the listening protocol to UDP and configures the receiver to use the
    /// specified endpoint. Ensure that the provided <paramref name="listenEndpoint"/> is valid and properly configured
    /// for UDP communication.</remarks>
    /// <param name="listenEndpoint">The <see cref="EndPoint"/> on which the receiver will listen for incoming UDP packets. Cannot be null.</param>
    protected UdpReceiver(EndPoint listenEndpoint, int socketBufferItems = default)
    {
        ListenProtocol = System.Net.Sockets.ProtocolType.Udp;
        ListenEndpoint = listenEndpoint;
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ListenProtocol);
        _socket.Bind(ListenEndpoint);
        _channel = socketBufferItems == default ? Channel.CreateUnbounded<Packet>() : Channel.CreateBounded<Packet>(socketBufferItems);
        _stopWatch = new Stopwatch();
    }

    /// <summary>
    /// Initializes and starts the receiver, binding it to the configured endpoint and preparing it to process incoming
    /// data.
    /// </summary>
    /// <remarks>This method sets up the necessary resources for receiving data, including initializing the
    /// socket and event arguments. If the receiver is already initialized, the method logs a warning and exits without
    /// reinitializing.</remarks>
    public virtual async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _stopWatch.Start();
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var receiverTask = Task.Run(() => ReceiveDataAndProcess(_cts.Token), cancellationToken);
        var processorTask = Task.Run(() => ProcessReceivedData(_cts.Token), cancellationToken);

        await Task.WhenAll(receiverTask, processorTask);
    }

    public virtual async Task StopAsync()
    {
        _stopWatch.Stop();
        var statistics = $@"
****** Server Statistics ****
Messages Received: {Interlocked.Read(ref _messagesReceived)}
Bytes Received: {Interlocked.Read(ref _bytesReceived)}
Elapsed Time: {_stopWatch.Elapsed}
*******************************";
        Logger.LogInformation("{Statistics}", statistics);

        if (_cts is not null)
        {
            await _cts.CancelAsync();
            _socket.Close();
        }
    }

    /// <summary>
    /// Attempts to parse the specified IP packet and extract the associated data of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>This method is abstract and must be implemented by a derived class. The implementation should
    /// define the specific parsing logic for the type <typeparamref name="T"/>.</remarks>
    /// <param name="packet">The IP packet to parse. Must not be <see langword="null"/>.</param>
    /// <param name="data">When this method returns <see langword="true"/>, contains the parsed data of type <typeparamref name="T"/>. When
    /// this method returns <see langword="false"/>, the value is <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the packet was successfully parsed and the data was extracted; otherwise, <see
    /// langword="false"/>.</returns>
    protected abstract bool TryParsePacket(TransportPacket packet, [NotNullWhen(true)] out T? data);

    private async Task ProcessReceivedData(CancellationToken cancellationToken)
    {
        if (_disposed)
            return;

        await foreach(var ipPacket in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            if (!ipPacket.PayloadPacketOrData.Value.TryPickT0(out var tp, out var _) || tp is not TransportPacket transportPacket)
            {
                Logger.LogError("Unable to process received data. Payload is not a transport packet.");
                throw new InvalidOperationException("Payload is not a transport packet.");
            }

            if(TryParsePacket(transportPacket, out var packet))
            {
                Received?.Invoke(this, packet!);
            }
        }
    }

    private async Task ReceiveDataAndProcess(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var buffer = _bufferPool.Rent(8192);
            var result = await _socket.ReceiveFromAsync(buffer, SocketFlags.None, ListenEndpoint, cancellationToken);
            var packet = PacketParser.Parse(DateTimeOffset.UtcNow, buffer.AsMemory()[..result.ReceivedBytes]);
            await _channel.Writer.WriteAsync(packet, cancellationToken);
            _bufferPool.Return(buffer);
            Interlocked.Increment(ref _messagesReceived);
            Interlocked.Add(ref _bytesReceived, result.ReceivedBytes);
        }

        _channel.Writer.Complete();
    }

    /// <summary>
    /// Releases the resources used by the server, including the underlying socket and any managed disposables.
    /// </summary>
    /// <remarks>This method should be called when the server is no longer needed to ensure proper cleanup of
    /// resources. It shuts down the socket connection, disposes of managed resources, and logs the shutdown
    /// process.</remarks>
    /// <param name="disposing">A value indicating whether the method is being called explicitly to release both managed and unmanaged resources
    /// (<see langword="true"/>), or by the finalizer to release only unmanaged resources (<see langword="false"/>).</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        Logger.LogWarning("Server shutting down");

        try
        {
            _socket?.Shutdown(SocketShutdown.Both);
        }
        catch (SocketException ex)
        {
            Logger.LogError(ex, "Error during socket shutdown.");
        }
        finally
        {
            _socket?.Dispose();
            _socket = null!;
        }

        try
        {
            _receiveEventArgs?.Dispose();
            _receiveEventArgs = null;

            _cts?.Dispose();
        }
        catch
        {
            // Do nothing
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
