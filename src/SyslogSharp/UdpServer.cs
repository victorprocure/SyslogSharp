using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace SyslogSharp;
internal sealed class UdpServer : IDisposable
{
    private bool _isStarted;
    private CancellationTokenSource? _stoppingCts;
    private CancellationToken _stoppingToken;
    private SocketAsyncEventArgs? _receiveEventArgs;
    private SocketAsyncEventArgs? _sendEventArgs;
    private Socket? _socket;
    private bool _isSocketDisposed;
    private IPEndPoint? _receiveEndpoint;
    private bool _receiving;
    private readonly SocketBufferManager _bufferManager = new(ushort.MaxValue, 8192);

    public Guid Id { get; }
    public string Address { get; }
    public ushort Port { get; }
    public EndPoint? EndPoint { get; private set; }
    public ILogger<UdpServer> Logger { get; set; } = NullLogger<UdpServer>.Instance;

    public event EventHandler<SyslogEventArgs>? Received;

    public UdpServer(IPAddress address, ushort port) : this(new IPEndPoint(address, port)){ }
    public UdpServer(string address, ushort port) : this(new IPEndPoint(IPAddress.Parse(address), port)){ }
    public UdpServer(IPEndPoint endPoint) : this(endPoint as EndPoint, endPoint.Address.ToString(), checked((ushort)endPoint.Port)) { }
    private UdpServer(EndPoint endPoint, string address, ushort port)
    {
        Id = Guid.NewGuid();
        Address = address;
        Port = port;
        EndPoint = endPoint;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(!_isStarted, "UDP Server already started!");
        if (_isStarted)
        {
            Logger.LogWarning("Syslog UDP server has already been started.");
            return Task.CompletedTask;
        }

        Logger.LogInformation("Starting syslog UDP server...");
        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        Start(_stoppingCts.Token);
        return Task.CompletedTask;
    }

    private bool Start(CancellationToken cancellationToken)
    {
        _stoppingToken = cancellationToken;
        _receiveEventArgs = new SocketAsyncEventArgs();
        _receiveEventArgs.Completed += OnAsyncCompleted;
        _sendEventArgs = new SocketAsyncEventArgs();
        _sendEventArgs.Completed += OnAsyncCompleted;

        _socket = CreateSocket();
        _isSocketDisposed = false;
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, false);

        _socket.Bind(EndPoint!);
        EndPoint = _socket.LocalEndPoint;

        _receiveEndpoint = new IPEndPoint((EndPoint!.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
        _isStarted = true;

        TryReceive();

        Logger.LogDebug("Syslog UDP server started...");
        return true;
    }

    private void TryReceive()
    {
        try
        {
            if (_receiving || !_isStarted || _receiveEventArgs is null || _socket is null)
                return;

            _receiving = true;
            _receiveEventArgs.RemoteEndPoint = _receiveEndpoint;

            _bufferManager.SetBuffer(_receiveEventArgs);

            if (!_socket.ReceiveFromAsync(_receiveEventArgs))
                ProcessReceive(_receiveEventArgs);
        }
        catch (ObjectDisposedException) { }
    }

    private void OnAsyncCompleted(object? sender, SocketAsyncEventArgs e)
    {
        if (_isSocketDisposed) return;

        switch (e.LastOperation)
        {
            case SocketAsyncOperation.ReceiveFrom:
                ProcessReceive(e);
                break;
        }
    }

    private void ProcessReceive(SocketAsyncEventArgs e)
    {
        _receiving = false;
        if (!_isStarted)
            return;

        if(e.SocketError != SocketError.Success)
        {
            Logger.LogError("A socket error occurred: {SocketError}", e.SocketError);

            OnReceived(e.RemoteEndPoint, e.Buffer, 0, 0);
            _bufferManager.FreeBuffer(e);
            TryReceive();
            return;
        }

        OnReceived(e.RemoteEndPoint, e.Buffer, e.Offset, e.BytesTransferred);
        _bufferManager.FreeBuffer(e);
        TryReceive();
    }

    private void OnReceived(EndPoint? remoteEndPoint, byte[]? buffer, int offset, int count)
    {
        if (buffer is null || buffer.Length == 0 || count == 0)
            return;

        var message = System.Text.Encoding.UTF8.GetString(buffer, offset, count);
        var syslogEventArgs = new SyslogEventArgs(message, remoteEndPoint?.Serialize().ToString(), null, null, null, null);
        Received?.Invoke(this, syslogEventArgs);
    }

    private Socket CreateSocket() => new(EndPoint!.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

    public void Dispose()
    {
        if (_isSocketDisposed)
        {
            return;
        }

        if(_receiveEventArgs is not null)
            _receiveEventArgs.Completed -= OnAsyncCompleted;

        if(_sendEventArgs is not null)
            _sendEventArgs.Completed -= OnAsyncCompleted;

        try
        {
            _socket?.Close();
            _socket?.Dispose();
            _receiveEventArgs?.Dispose();
            _sendEventArgs?.Dispose();
            _isSocketDisposed = true;
        }
        catch (ObjectDisposedException)
        {
            // Do nothing
        }

        _isStarted = false;
        Logger.LogInformation("UDP Syslog server stopped");
    }
}
