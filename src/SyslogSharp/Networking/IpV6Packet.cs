using System.Net;

namespace SyslogSharp.Networking;

internal sealed record IpV6Packet : IpPacket
{
    private const int IpV6HeaderLength = 40;
    private readonly IPAddress _sourceAddress;
    private readonly IPAddress _destinationAddress;
    private readonly ushort _totalLength;

    public IpV6Packet(ArraySegment<byte> packetData, Packet? parent = default, DateTimeOffset receivedTime = default)
        :base(parent, receivedTime)
    {
        if (packetData.Count < IpV6HeaderLength)
            throw new ArgumentException("Invalid IPv6 packet data");

        var version = (byte)(packetData[0] >> 4);
        ArgumentOutOfRangeException.ThrowIfNotEqual(version, 6);
        ReceivedAt = receivedTime == default ? DateTimeOffset.UtcNow : receivedTime;
        TrafficClass = (byte)(((packetData[0] & 0x0F) << 4) | (packetData[1] >> 4));
        FlowLabel = (uint)(((packetData[1] & 0x0F) << 16) | (packetData[2] << 8) | packetData[3]);

        PayloadLength = (ushort)((packetData[4] << 8) | packetData[5]);
        NextHeader = (ProtocolType)packetData[6];
        HopLimit = packetData[7];

        _sourceAddress = new IPAddress(packetData.Slice(8, 16));
        _destinationAddress = new IPAddress(packetData.Slice(24, 16));

        // Parse extension headers
        var headerOffset = packetData.Offset + IpV6HeaderLength;
        var currentHeader = NextHeader;
        var extensionStart = headerOffset;
        var extensionEnd = extensionStart;
        var data = packetData.Array!;

        while (extensionEnd < packetData.Offset + packetData.Count && IsExtensionHeader(currentHeader))
        {
            var extHeaderStart = extensionEnd;
            var nextHeaderValue = data[extHeaderStart];
            var hdrExtLen = data[extHeaderStart + 1];

            var extHeaderLen = currentHeader switch
            {
                ProtocolType.IPv6_Frag => 8,
                ProtocolType.AH => (hdrExtLen + 2) * 4,
                _ => (hdrExtLen + 1) * 8
            };

            extensionEnd += extHeaderLen;
            currentHeader = (ProtocolType)nextHeaderValue;

            if (currentHeader == ProtocolType.IPv6_NoNxt || extensionEnd >= packetData.Offset + packetData.Count)
            {
                break;
            }
        }

        ExtensionHeaders = default;
        if (extensionEnd > extensionStart)
        {
            ExtensionHeaders = new ArraySegment<byte>(data, extensionStart, extensionEnd - extensionStart);
        }

        var payloadStart = extensionEnd;
        PayloadLength = (ushort)Math.Max(0, packetData.Offset + packetData.Count - payloadStart);
        Protocol = currentHeader;

        if (PayloadLength > 0)
        {
            PayloadPacketOrData = new(() => {
                var payload = new ArraySegment<byte>(data, payloadStart, PayloadLength);

                return ParsePayload(payload, currentHeader, this);
            });
        }

        _totalLength = (ushort)(PayloadLength + IpV6HeaderLength);
    }

    public RawIpPacketProtocol IpProtocol { get; } = RawIpPacketProtocol.IpV6;
    public byte TrafficClass { get; set; }
    public uint FlowLabel { get; set; }
    public ushort PayloadLength { get; set; }
    public ProtocolType NextHeader { get; set; }
    public byte HopLimit { get; set; }
    public ArraySegment<byte> ExtensionHeaders { get; set; }

    public override IPAddress DestinationAddress => _destinationAddress;
    public override IPAddress SourceAddress => _sourceAddress;
    protected override ushort TotalLength => _totalLength;

    /// <summary>
    /// Determines whether the specified protocol type is an IPv6 extension header.
    /// </summary>
    /// <remarks>IPv6 extension headers are used to provide additional information or functionality for IPv6
    /// packets. This method checks if the given protocol type corresponds to one of the known IPv6 extension headers,
    /// such as Hop-by-Hop Options, Routing, Fragment, Encapsulating Security Payload (ESP), Authentication Header (AH),
    /// No Next Header, Destination Options, or Mobility Header.</remarks>
    /// <param name="protocolTypeNumber">The protocol type to evaluate.</param>
    /// <returns><see langword="true"/> if the specified protocol type is an IPv6 extension header; otherwise, <see
    /// langword="false"/>.</returns>
    private static bool IsExtensionHeader(ProtocolType protocolTypeNumber)
    {
        return protocolTypeNumber switch
        {
            ProtocolType.HOPOPT or // 0
            ProtocolType.IPv6_Route or // 43
            ProtocolType.IPv6_Frag or // 44
            ProtocolType.ESP or // 50
            ProtocolType.AH or // 51
            ProtocolType.IPv6_NoNxt or // 59
            ProtocolType.IPv6_Opts or // 60
            ProtocolType.Mobility_Header => true, // 135
            _ => false
        };
    }
}
