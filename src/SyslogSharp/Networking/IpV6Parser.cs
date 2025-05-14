using System.Net;

namespace SyslogSharp.Networking;

/// <summary>
/// Parses raw IPv6 packet data into a structured <see cref="IpV6Packet"/> object.
/// </summary>
/// <remarks>This method parses the IPv6 header, including fields such as version, traffic class, flow label,
/// payload length, next header, hop limit, source address, and destination address. It also processes any extension
/// headers and extracts the payload data.  The method assumes that the provided <paramref name="packetData"/> is
/// well-formed and contains valid IPv6 packet data. If the packet includes extension headers, they are parsed and
/// included in the resulting <see cref="IpV6Packet"/> object.</remarks>
internal static class IpV6Parser
{
    /// <summary>
    /// Parses raw IPv6 packet data into an <see cref="IpV6Packet"/> instance.
    /// </summary>
    /// <remarks>This method parses the IPv6 header, extension headers (if present), and the payload of the
    /// packet. If <paramref name="reuseBuffer"/> is set to <see langword="true"/>, the returned <see
    /// cref="IpV6Packet"/> will reference the original <paramref name="packetData"/> buffer for its data. If set to
    /// <see langword="false"/>, new buffers will be allocated for the extension headers and payload.</remarks>
    /// <param name="receivedTime">The timestamp indicating when the packet was received.</param>
    /// <param name="packetData">A segment of bytes containing the raw IPv6 packet data. The segment must include at least the IPv6 header.</param>
    /// <param name="reuseBuffer">A boolean value indicating whether the internal buffers of the parsed packet should reference the original
    /// <paramref name="packetData"/> buffer (<see langword="true"/>), or if new buffers should be allocated (<see
    /// langword="false"/>).</param>
    /// <returns>An <see cref="IpV6Packet"/> instance representing the parsed IPv6 packet, including its headers and payload.</returns>
    /// <exception cref="ArgumentException">Thrown if <paramref name="packetData"/> does not contain enough data to include a valid IPv6 header.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if IP packet version is not 6</exception>
    public static IpV6Packet Parse(DateTimeOffset receivedTime, ArraySegment<byte> packetData, bool reuseBuffer)
    {
        if (packetData.Count < IpV6PacketConstants.Ipv6HeaderLength)
            throw new ArgumentException("Invalid IPv6 packet data");

        var data = packetData.Array!;
        var offset = packetData.Offset;

        var version = (byte)(packetData[0] >> 4);
        ArgumentOutOfRangeException.ThrowIfNotEqual(version, 6);

        var trafficClass = (byte)(((packetData[0] & 0x0F) << 4) | (packetData[1] >> 4));
        uint flowLabel = (uint)(((packetData[1] & 0x0F) << 16) | (packetData[2] << 8) | packetData[3]);

        var payloadLength = (ushort)((packetData[4] << 8) | packetData[5]);
        var nextHeader = (ProtocolType)packetData[6];
        var hopLimit = packetData[7];

        var sourceAddress = new IPAddress(packetData.Slice(8, 16));
        var destinationAddress = new IPAddress(packetData.Slice(24, 16));

        // Parse extension headers
        var headerOffset = offset + IpV6PacketConstants.Ipv6HeaderLength;
        var currentHeader = nextHeader;
        var extensionStart = headerOffset;
        var extensionEnd = extensionStart;

        while (extensionEnd < offset + packetData.Count && IsExtensionHeader(currentHeader))
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

            if (currentHeader == ProtocolType.IPv6_NoNxt || extensionEnd >= offset + packetData.Count)
            {
                break;
            }
        }

        var extensionHeaders = default(ArraySegment<byte>);
        if (extensionEnd > extensionStart)
        {
            extensionHeaders = new ArraySegment<byte>(data, extensionStart, extensionEnd - extensionStart);
            if (!reuseBuffer)
            {
                extensionHeaders = new ArraySegment<byte>([.. extensionHeaders]);
            }
        }

        var payloadStart = extensionEnd;
        var payloadLengthActual = Math.Max(0, offset + packetData.Count - payloadStart);

        var payload = default(ArraySegment<byte>);
        if (payloadLengthActual > 0)
        {
            payload = new ArraySegment<byte>(data, payloadStart, payloadLengthActual);
            if (!reuseBuffer)
            {
                payload = new ArraySegment<byte>([.. payload]);
            }
        }

        return new IpV6Packet
        {
            Version = version,
            TrafficClass = trafficClass,
            FlowLabel = flowLabel,
            PayloadLength = payloadLength,
            NextHeader = nextHeader,
            HopLimit = hopLimit,
            SourceAddress = sourceAddress,
            DestinationAddress = destinationAddress,
            ExtensionHeaders = extensionHeaders,
            Payload = payload,
            ReceivedTime = receivedTime
        };
    }

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

    /// <summary>
    /// Provides constant values used for parsing and constructing IPv6 packet headers.
    /// </summary>
    private static class IpV6PacketConstants
    {
        public const int Ipv6HeaderLength = 40;
    }
}


