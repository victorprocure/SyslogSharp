using Microsoft.Extensions.Logging;

namespace SyslogSharp.Writers;
internal sealed class SyslogLoggerWriter : ISyslogWriter
{
    private readonly ILogger<SyslogLoggerWriter> _logger;

    public SyslogLoggerWriter(ILogger<SyslogLoggerWriter> logger) => _logger = logger;

    public Task WriteSyslogMessage(SyslogEventArgs syslogEventArgs)
    {
        _logger.LogInformation("Syslog message received: {message}, from: {hostname}", syslogEventArgs.Message, syslogEventArgs.Hostname);

        return Task.CompletedTask;
    }
}
