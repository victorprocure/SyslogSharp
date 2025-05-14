namespace SyslogSharp.Networking;
internal sealed record UdpDatagram
{
    public required IpPacketHeader PacketHeader { get; init; }
    public required UdpDatagramHeader? UdpDatagramHeader { get; init; }
    public required DateTimeOffset ReceivedTime { get; init; }
    public ArraySegment<byte> UdpData { get; init; }
}
