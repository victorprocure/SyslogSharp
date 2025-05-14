namespace SyslogSharp;
internal sealed record SyslogMessage
{
    public DateTimeOffset OccurrenceTime { get; internal set; }

    public DateTimeOffset ReceivedTime { get; internal set; }

    public byte[]? Payload { get; internal set; }

    public SyslogEvent? PayloadInstance { get; internal set; }

    public string Source => string.Empty;

    public static string Protocol => "Syslog";

    public static string TypeId { get; } = typeof(SyslogEvent).GetTypeIdentifier();
}
