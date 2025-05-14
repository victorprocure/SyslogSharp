namespace SyslogSharp.Networking;
internal sealed record UdpDatagramHeader
{
    public UdpDatagramHeader(ushort sourcePort, ushort destinationPort, ushort udpLength, ushort udpCheckSum)
    {
        SourcePort = sourcePort;
        DestinationPort = destinationPort;
        UdpLength = udpLength;
        UdpCheckSum = udpCheckSum;
    }

    public ushort SourcePort { get; }
    public ushort DestinationPort { get; }
    public ushort UdpLength { get; }
    public ushort UdpCheckSum { get; }
}
