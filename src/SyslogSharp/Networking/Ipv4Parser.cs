using System.Net;
using System.Net.Sockets;

namespace SyslogSharp.Networking;

internal static class Ipv4Parser
{
    public static IpV4Packet Parse(DateTimeOffset receivedTime, ArraySegment<byte> packet, bool reuseBuffer)
    {
        var ipVersion = (byte)(packet[0] >> 4);
        if (ipVersion != 4)
            throw new NotSupportedException($"IP version {ipVersion} is not supported.");

        var ipV4Packet = new IpV4Packet()
        {
            Version = ipVersion,
            IHL = (byte)(packet[0] & 0x0F),
            ReceivedAt = receivedTime
        };

        var headerLength = ipV4Packet.IHL * 4;
        if (headerLength < 20 || packet.Count < headerLength)
            throw new ArgumentOutOfRangeException(nameof(packet), "Header length is less than 20 bytes.");

        ipV4Packet.DSCP = (byte)(packet[1] >> 2);
        ipV4Packet.ECN = (byte)(packet[1] & 0x03);
        ipV4Packet.TotalLength = (ushort)((packet[2] << 8) | packet[3]);
        ipV4Packet.Identification = (ushort)((packet[4] << 8) | packet[5]);
        ipV4Packet.HeaderFlags = (ushort)((packet[6] << 8) | packet[7]);
        ipV4Packet.DF = (ipV4Packet.HeaderFlags & 0x4000) != 0;
        ipV4Packet.MF = (ipV4Packet.HeaderFlags & 0x2000) != 0;
        ipV4Packet.FragmentOffset = (ushort)(ipV4Packet.HeaderFlags & 0x1FFF);
        ipV4Packet.TTL = packet[8];
        ipV4Packet.Protocol = (ProtocolType)packet[9];
        ipV4Packet.HeaderChecksum = (ushort)((packet[10] << 8) | packet[11]);
        ipV4Packet.SourceAddress = new IPAddress(packet.Slice(12, 4));
        ipV4Packet.DestinationAddress = new IPAddress(packet.Slice(16, 4));

        ipV4Packet.Options = [];
        if (headerLength > 20)
        {
            var length = headerLength - 20;
            if (reuseBuffer)
            {
                ipV4Packet.Options = packet.Slice(20, length);
            }
            else
            {
                var options = packet.Slice(20, length).ToArray();
                ipV4Packet.Options = new(options);
            }
        }

        ipV4Packet.Payload = [];
        if (packet.Count > headerLength)
        {
            var length = ipV4Packet.TotalLength - headerLength;
            if (reuseBuffer)
            {
                ipV4Packet.Payload = packet.Slice(headerLength, length);
            }
            else
            {
                var payload = packet.Slice(headerLength, length).ToArray();
                ipV4Packet.Payload = new(payload);
            }
        }

        return ipV4Packet;
    }

    public static IpV4Packet Parse(DateTimeOffset receivedTime, bool reuseBuffer, byte[]? packetBytes, int offset = 0)
    {
        ArgumentNullException.ThrowIfNull(packetBytes);
        var byteArraySegment = new ArraySegment<byte>(packetBytes, offset, packetBytes.Length - offset);

        return Parse(receivedTime, byteArraySegment, reuseBuffer);
    }
}
