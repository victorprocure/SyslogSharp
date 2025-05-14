using System.Reflection;
using System.Text.Json.Serialization;

namespace SyslogSharp;

internal sealed record SyslogSettings
{
    [JsonIgnore]
    public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";

    /// <summary>
    /// Gets the UDP port to listen for syslog events on.
    /// </summary>
    /// <remarks>Default is 514</remarks>
    public ushort UdpPort { get; set; } = 514;

    /// <summary>
    /// Gets the TCP port to listen for syslog events on.
    /// </summary>
    /// <remarks>Default is 6514</remarks>
    public ushort TcpPort { get; set; } = 6514;

    /// <summary>
    /// Gets whether to listen using secure TCP (TLS) or not.
    /// </summary>
    public bool UseTcp { get; set; }

    /// <summary>
    /// Gets the IP Address to listen for syslog events on.
    /// </summary>
    /// <remarks>If null or empty this will default to all interfaces: "0.0.0.0"</remarks>
    public string? IpAddress { get; set; }
}
