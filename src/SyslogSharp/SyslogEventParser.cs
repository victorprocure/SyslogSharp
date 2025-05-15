using System.Text;
using System.Text.RegularExpressions;

namespace SyslogSharp;

/// <summary>
/// Parses syslog messages into structured <see cref="SyslogEvent"/> objects.
/// </summary>
/// <remarks>This class provides functionality to parse syslog messages conforming to various standards, such as
/// BSD (RFC 3164) and RFC 5424. It uses regular expressions to extract key components of the syslog message, including
/// priority, severity, facility, and message content. <para> The <see cref="Parse"/> method is the primary entry point
/// for parsing syslog messages. It validates the input, matches the message against the expected format, and extracts
/// relevant fields into a <see cref="SyslogEvent"/> object. </para> <para> This class is designed to handle syslog
/// messages received over the network, with support for UTF-8 encoded messages. </para></remarks>
internal sealed partial class SyslogEventParser
{
    private readonly string[] _groupNames;

    [GeneratedRegex(@"\<(?<PRIVAL>\d+?)\>\s*(?<MESSAGE>.+)")]
    private static partial Regex SyslogDefaultParser();

    /// <summary>
    /// Provides a regular expression for parsing BSD syslog messages.
    /// </summary>
    /// <remarks>The regular expression is designed to extract key components of a BSD syslog message,
    /// including the priority (PRI), timestamp, hostname, and message content. The timestamp is further broken down
    /// into its constituent parts (month, day, hour, minute, and second). This method generates a compiled regular
    /// expression for efficient matching. This follows the RFC3164 standard</remarks>
    /// <returns>A <see cref="Regex"/> instance that matches BSD syslog messages and provides named groups for extracting their
    /// components.</returns>
    [GeneratedRegex(@"
(\<(?<PRI>\d{1,3})\>){0,1}
(?<HDR>
  (?<TIMESTAMP>(?<MMM>Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\s
               (?<DD>[ 0-9][0-9])\s
	           (?<HH>[0-9]{2})\:(?<MM>[0-9]{2})\:(?<SS>[0-9]{2})
  )\s
  (?<HOSTNAME>[^ ]+?)\s
){0,1}
(?<MSG>.*)
")]
    private static partial Regex BsdSyslogParser();

    /// <summary>
    /// Provides a regular expression for parsing syslog messages that conform to the RFC 5424 standard.
    /// </summary>
    /// <remarks>This method generates a compiled regular expression that matches the structure of an RFC 5424
    /// syslog message. The pattern captures key components of the syslog message, including priority, version,
    /// timestamp, hostname,  application name, process ID, message ID, structured data, and the message body. <para>
    /// The named groups in the regular expression correspond to the components of the syslog message: <list
    /// type="bullet"> <item><description><c>PRI</c>: The priority value enclosed in angle
    /// brackets.</description></item> <item><description><c>VER</c>: The version of the syslog
    /// protocol.</description></item> <item><description><c>TIMESTAMP</c>: The timestamp of the message, including
    /// optional time zone information.</description></item> <item><description><c>HOSTNAME</c>: The hostname or IP
    /// address of the sender.</description></item> <item><description><c>APPNAME</c>: The name of the application that
    /// generated the message.</description></item> <item><description><c>PROCID</c>: The process ID of the
    /// application.</description></item> <item><description><c>MSGID</c>: The message ID.</description></item>
    /// <item><description><c>SD</c>: Structured data associated with the message.</description></item>
    /// <item><description><c>MSG</c>: The message body.</description></item> </list> </para> This method is marked as
    /// <see langword="partial"/> and <see cref="GeneratedRegexAttribute"/> is used to generate the compiled regular
    /// expression at compile time for improved performance.</remarks>
    /// <returns>A <see cref="Regex"/> object that matches RFC 5424 syslog messages.</returns>
    [GeneratedRegex(@"
(?<HDR>
	(\<(?<PRI>\d{1,3})\>){0,1}
	(?<VER>\d{1})\s
	((?<TIMESTAMP>(?<Year>[0-9]{4})-(?<Month>[0-9]{2})-(?<Day>[0-9]{2})
        ([Tt](?<HH>[0-9]{2}):(?<MM>[0-9]{2}):(?<SS>[0-9]{2})(\.[0-9]+){0,1})?
        ([Tt](?<HH>[0-9]{2}):(?<MM>[0-9]{2}):(?<SS>[0-9]{2})(\\.[0-9]+){0,1})?
        (([Zz]|(?<OffsetSign>[+-])(?<OffsetHours>[0-9]{2}):(?<OffsetMinutes>[0-9]{2})))?
	)|(?<NIL>-))\s
	((?<HOSTNAME>[^ ]+?)|(?<NIL>-))\s
	((?<APPNAME>[^ ]+?)|(?<NIL>-))\s
	((?<PROCID>[^ ]+?)|(?<NIL>-))\s
	((?<MSGID>[^ ]+?)|(?<NIL>-))\s
	((?<SD>(\[.*\])+)|(?<NIL>-))	
)
(\s(?<MSG>.*))?
")]
    private static partial Regex Rfc5424SyslogParser();

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
