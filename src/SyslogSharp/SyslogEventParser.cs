using System.Text;
using System.Text.RegularExpressions;

namespace SyslogSharp;
internal sealed partial class SyslogEventParser
{
    private readonly string[] _groupNames;

    [GeneratedRegex(@"\<(?<PRIVAL>\d+?)\>\s*(?<MESSAGE>.+)")]
    private static partial Regex SyslogDefaultParser();

    public SyslogEventParser()
    {
        _groupNames = SyslogDefaultParser().GetGroupNames();
    }

    public SyslogEvent Parse(ArraySegment<byte> segment, DateTimeOffset receivedTime, string sourceIpAddress)
    {
        if(segment.Array is null)
        {
            throw new ArgumentNullException(nameof(segment));
        }

        var logMessage = Encoding.UTF8.GetString(segment.Array, segment.Offset, segment.Count);
        if(string.IsNullOrEmpty(logMessage))
        {
            throw new ArgumentException("Incoming log message had no data.", nameof(segment));
        }

        var defMatch = SyslogDefaultParser().Match(logMessage);
        if(!defMatch.Success)
        {
            throw new FormatException($"Incoming log message did not match expected format: {logMessage}");
        }

        if(defMatch.Groups.Count < 1)
        {
            throw new FormatException($"Incoming log message did not contain expected groups: {logMessage}");
        }

        var priorityValueMatch = defMatch.Groups["PRIVAL"].Value.Trim();

        if(string.IsNullOrWhiteSpace(priorityValueMatch))
        {
            throw new FormatException($"Incoming log message did not contain expected priority value: {logMessage}");
        }

        var priorityValue = int.Parse(priorityValueMatch);
        var severity = (SyslogSeverity)Enum.ToObject(typeof(SyslogSeverity), priorityValue & 0x7);
        var facility = (SyslogFacility)Enum.ToObject(typeof(SyslogFacility), priorityValue >> 3);
        var message = defMatch.Groups["MESSAGE"].Value.Trim();

        var matches = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach(var groupName in _groupNames)
        {
            var group = defMatch.Groups[groupName];
            if(group.Success && !string.IsNullOrEmpty(group.Value))
            {
                matches[groupName] = group.Value;
            }
        }

        return new(receivedTime, sourceIpAddress, severity, facility, message, matches);
    }
}
