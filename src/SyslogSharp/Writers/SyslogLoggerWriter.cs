using Microsoft.Extensions.Logging;

namespace SyslogSharp.Writers;
internal sealed class SyslogLoggerWriter : ISyslogWriter
{
    private readonly ILogger<SyslogLoggerWriter> _logger;

    public SyslogLoggerWriter(ILogger<SyslogLoggerWriter> logger) => _logger = logger;

    public Task WriteSyslogMessage(SyslogEvent syslogEventArgs)
    {
        var level = syslogEventArgs.Severity switch
        {
            SyslogSeverity.Debug => LogLevel.Debug,
            SyslogSeverity.Informational => LogLevel.Information,
            SyslogSeverity.Notice => LogLevel.Information,
            SyslogSeverity.Warning => LogLevel.Warning,
            SyslogSeverity.Error => LogLevel.Error,
            SyslogSeverity.Critical => LogLevel.Critical,
            _ => LogLevel.Information
        };

        _logger.Log(level, "Syslog message received: {Message}, from: {Hostname}", syslogEventArgs.Message, syslogEventArgs.SourceIpAddress);

        return Task.CompletedTask;
    }
}
