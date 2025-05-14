
namespace SyslogSharp;
public sealed record SyslogEvent
{
    internal SyslogEvent(DateTimeOffset receivedTime, string sourceIpAddress, SyslogSeverity severity, SyslogFacility facility, string message, Dictionary<string, string> matches)
    {
        ReceivedTime = receivedTime;
        SourceIpAddress = sourceIpAddress;
        Severity = severity;
        Facility = facility;
        Message = message;
        Matches = matches;
    }

    public DateTimeOffset ReceivedTime { get; }
    public string SourceIpAddress { get; }
    public SyslogSeverity Severity { get; }
    public SyslogFacility Facility { get; }
    public string Message { get; }
    public Dictionary<string, string> Matches { get; }
}
