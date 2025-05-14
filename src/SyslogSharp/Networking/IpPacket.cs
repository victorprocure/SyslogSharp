using System.Net.Sockets;

namespace SyslogSharp.Networking;
internal sealed record IpPacket
{
    public required IpPacketHeader PacketHeader { get; init; }
    public required DateTimeOffset ReceivedTime { get; init; }
    public required ProtocolType ProtocolType { get; init; }
    public ArraySegment<byte> IpOptions { get; init; }
    public ArraySegment<byte> PacketData { get; init; }
}
