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
        _stoppingCts?.Dispose();
        _udpServer?.Dispose();
    }

    public async Task StartServerAsync(CancellationToken cancellationToken)
    {
        var settings = await _settingsParser.ParseAsync(cancellationToken);
        _logger.LogInformation("SyslogSharp Version: {Version}", SyslogSettings.Version);
        _logger.LogInformation("Server IP: {IpAddress}", settings.IpAddress ?? "0.0.0.0");
        _logger.LogInformation("Server Port: {Port}", settings.UseTcp ? settings.TcpPort : settings.UdpPort);

        _stoppingCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await StartListenerAsync(settings, _stoppingCts.Token);
    }

    public async Task StopServerAsync()
    {
        try
        {
            await _stoppingCts!.CancelAsync();
            if (_udpServer is not null)
            {
                await _udpServer.StopAsync();
                _udpServer.Received -= OnSyslogMessageReceived;
                _udpServer?.Dispose();
                _udpServer = null;
            }
        }
        catch
        {
            // Ignore exceptions during shutdown
        }
    }

    private async Task StartListenerAsync(SyslogSettings settings, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting syslog server...");
        _udpServer = new UdpServer(IPAddress.Any, settings.UdpPort)
        {
            Logger = _loggerFactory.CreateLogger<UdpServer>()
        };

        _udpServer.Received += OnSyslogMessageReceived;

        _ = _udpServer.StartAsync(cancellationToken);

        await Task.CompletedTask;
    }

    private async void OnSyslogMessageReceived(object? sender, SyslogMessage message)
    {
        if(message.PayloadInstance is null)
        {
            return;
        }

        await _syslogWriter.WriteSyslogMessage(message.PayloadInstance).ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);
    }
}
