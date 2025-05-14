using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Concurrent;

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
    private Socket? _socket;
    private bool _disposed;
    private ConcurrentQueue<SocketAsyncEventArgs>? _receiveDataProcessorsPool;
    private readonly List<IDisposable> _disposables = [];
    private readonly int _concurrentReceivers;

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
    protected UdpReceiver(EndPoint listenEndpoint, int concurrentReceivers = 10)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(concurrentReceivers, 1);

        ListenProtocol = System.Net.Sockets.ProtocolType.Udp;
        ListenEndpoint = listenEndpoint;
        _concurrentReceivers = concurrentReceivers;
    }

    /// <summary>
    /// Initializes and starts the receiver, binding it to the configured endpoint and preparing it to process incoming
    /// data.
    /// </summary>
    /// <remarks>This method sets up the necessary resources for receiving data, including initializing the
    /// socket and event arguments. If the receiver is already initialized, the method logs a warning and exits without
    /// reinitializing.</remarks>
    public void Start()
    {
        if (_socket != null || _receiveDataProcessorsPool != null)
        {
            Logger.LogWarning("Start called on an already initialized receiver.");
            return;
        }

        _receiveDataProcessorsPool = new ConcurrentQueue<SocketAsyncEventArgs>();
        var eventArgsHandler = new EventHandler<SocketAsyncEventArgs>(ReceiveCompletedHandler);

        for(var i = 0; i < _concurrentReceivers; i++)
        {
            var eventArgs = new SocketAsyncEventArgs();
            eventArgs.SetBuffer(new byte[ushort.MaxValue], 0, ushort.MaxValue);
            eventArgs.Completed += eventArgsHandler;
            _receiveDataProcessorsPool.Enqueue(eventArgs);
            _disposables.Add(eventArgs);
        }

        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, System.Net.Sockets.ProtocolType.Udp)
        {
            ReceiveBufferSize = int.MaxValue
        };

        _socket.Bind(ListenEndpoint);

        ReceiveDataAndProcess();
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

    private void ReceiveCompletedHandler(object? sender, SocketAsyncEventArgs e)
    {
        if (_disposed)
            return;

        if (e.LastOperation == SocketAsyncOperation.Receive || e.LastOperation == SocketAsyncOperation.ReceiveFrom)
            ProcessReceivedData(e);
    }

    private void ProcessReceivedData(SocketAsyncEventArgs e)
    {
        if (_disposed)
            return;

        if (e.Buffer is null)
        {
            Logger.LogError("Unable to process received data. Buffer is null.");
            throw new InvalidOperationException("Buffer is null.");
        }

        var ipPacket = PacketParser.Parse(DateTimeOffset.UtcNow, false, e.Buffer, 0);
        var payload = ipPacket.PayloadPacketOrData.Value.AsT0;

        if (e.SocketError == SocketError.Success && payload is TransportPacket transportPacket && TryParsePacket(transportPacket, out var packet))
        {
            Received?.Invoke(this, packet!);
        }

        e.SetBuffer(0, ushort.MaxValue);
        _receiveDataProcessorsPool?.Enqueue(e);
        ReceiveDataAndProcess();
    }

    private void ReceiveDataAndProcess()
    {
        if (_disposed)
            return;

        if (_socket is null || _receiveDataProcessorsPool is null)
        {
            Logger.LogError("Receiver not initialized correctly.");
            throw new InvalidOperationException("Receiver not initialized correctly.");
        }

        if(_receiveDataProcessorsPool.TryDequeue(out var deqAsyncEvent))
        {
            if(deqAsyncEvent.Offset != 0 || deqAsyncEvent.Count != ushort.MaxValue)
            {
                deqAsyncEvent.SetBuffer(new byte[ushort.MaxValue], 0, ushort.MaxValue);
            }

            if(!_socket.ReceiveAsync(deqAsyncEvent))
                ProcessReceivedData(deqAsyncEvent);
        }
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
            _socket = null;
        }

        try
        {
            foreach(var disposable in _disposables)
            {
                disposable.Dispose();
            }
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
