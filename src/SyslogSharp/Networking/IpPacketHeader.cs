using System.Net;

namespace SyslogSharp.Networking;
internal readonly record struct IpPacketHeader
{
    public IpPacketHeader(
            IPAddress sourceIpAddress,
            IPAddress destinationIpAddress,
            bool isIp6,
            byte internetHeaderLength,
            byte dscpValue,
            byte explicitCongestionNotice,
            ushort ipPacketLength,
            ushort fragmentGroupId,
            ushort ipHeaderFlags,
            ushort fragmentOffset,
            byte timeToLive,
            ushort packetHeaderChecksum)
    {
        DestinationIpAddress = destinationIpAddress;
        IsIp6 = isIp6;
        InternetHeaderLength = internetHeaderLength;
        DscpValue = dscpValue;
        ExplicitCongestionNotice = explicitCongestionNotice;
        IpPacketLength = ipPacketLength;
        FragmentGroupId = fragmentGroupId;
        IpHeaderFlags = ipHeaderFlags;
        FragmentOffset = fragmentOffset;
        TimeToLive = timeToLive;
        PacketHeaderChecksum = packetHeaderChecksum;
        SourceIpAddress = sourceIpAddress;
    }

    public IPAddress SourceIpAddress { get; }
    public IPAddress DestinationIpAddress { get; }
    public bool IsIp6 { get; }
    public byte InternetHeaderLength { get; }
    public byte DscpValue { get; }
    public byte ExplicitCongestionNotice { get; }
    public ushort IpPacketLength { get; }
    public ushort FragmentGroupId { get; }
    public ushort IpHeaderFlags { get; }
    public ushort FragmentOffset { get; }
    public byte TimeToLive { get; }
    public ushort PacketHeaderChecksum { get; }
}
