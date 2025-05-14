using System.Net;

namespace SyslogSharp.Networking;
internal record struct IpV6Packet
{
    public byte Version { get; set; }
    public byte TrafficClass { get; set; }
    public uint FlowLabel { get; set; }
    public ushort PayloadLength { get; set; }
    public ProtocolType NextHeader { get; set; }
    public byte HopLimit { get; set; }
    public IPAddress SourceAddress { get; set; }
    public IPAddress DestinationAddress { get; set; }
    public ArraySegment<byte> ExtensionHeaders { get; set; }
    public ArraySegment<byte> Payload { get; set; }
    public DateTimeOffset ReceivedTime { get;  set; }
}
