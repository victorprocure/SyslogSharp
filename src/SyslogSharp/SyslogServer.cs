using System.Net;
using Microsoft.Extensions.Logging;
using SyslogSharp.Writers;

namespace SyslogSharp;
internal sealed class SyslogServer : IDisposable
{
    private readonly SyslogSettingsParser _settingsParser;
    private readonly ISyslogWriter _syslogWriter;
    private readonly ILogger<SyslogServer> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private CancellationTokenSource? _stoppingCts;
    private UdpServer? _udpServer;

    public SyslogServer(SyslogSettingsParser settingsParser, ISyslogWriter syslogWriter, ILogger<SyslogServer> logger, ILoggerFactory loggerFactory)
    {
        _settingsParser = settingsParser;
        _syslogWriter = syslogWriter;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public void Dispose()
    {
        _stoppingCts?.Cancel();
        _udpServer?.Dispose();
    }

    public async Task StartServerAsync(CancellationToken cancellationToken)
    {
        var settings = await _settingsParser.ParseAsync(cancellationToken);
        _logger.LogInformation("SyslogSharp Version: {version}", SyslogSettings.Version);
        _logger.LogInformation("Server IP: {ipAddress}", settings.IpAddress ?? "0.0.0.0");
        _logger.LogInformation("Server Port: {port}", settings.UseTcp ? settings.TcpPort : settings.UdpPort);

        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await StartListenerAsync(settings, _stoppingCts.Token);
    }

    public Task StopServerAsync(CancellationToken cancellationToken)
    {
        try
        {
            _stoppingCts!.Cancel();
            if (_udpServer is not null)
            {
                _udpServer.Received -= OnSyslogMessageReceived;
                _udpServer?.Dispose();
                _udpServer = null;
            }
        }
        catch { }

        return Task.CompletedTask;
    }

    private async Task StartListenerAsync(SyslogSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting syslog server...");
        _udpServer = new UdpServer(IPAddress.Any, settings.UdpPort)
        {
            Logger = _loggerFactory.CreateLogger<UdpServer>()
        };

        _udpServer.Received += OnSyslogMessageReceived;

        await _udpServer.StartAsync(cancellationToken);
    }

    private async void OnSyslogMessageReceived(object? sender, SyslogEventArgs message)
        => await _syslogWriter.WriteSyslogMessage(message).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
}
