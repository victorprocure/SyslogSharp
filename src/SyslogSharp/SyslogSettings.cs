using System.Reflection;
using System.Text.Json.Serialization;

namespace SyslogSharp;
internal sealed record SyslogSettings
{
    [JsonIgnore]
    public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
    public ushort UdpPort { get; set; } = 514;
    public ushort TcpPort { get; set; } = 6514;
    public bool UseTcp { get; set; }
    public string? IpAddress { get; set; }
}
