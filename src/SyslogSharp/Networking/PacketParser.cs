using System.Net.Sockets;

using static SyslogSharp.Networking.ParserHelpers;

namespace SyslogSharp.Networking;
internal static class PacketParser
{
    public static IpPacket Parse(ArraySegment<byte> data)
    {
        if(data.Count == 0)
        {
            throw new ArgumentException("Data cannot be an empty collection", nameof(data));
        }

        return Parse(DateTimeOffset.UtcNow, true, data.Array, data.Offset);
    }

    /// <summary>
    /// Parses a raw byte array representing an IPv4 packet into an <see cref="IpPacket"/> object.
    /// </summary>
    /// <param name="receivedTime">The timestamp when the packet was received.</param>
    /// <param name="reuseBuffer">
    /// A flag indicating whether to reuse the original buffer for <see cref="IpPacket.IpOptions"/> 
    /// and <see cref="IpPacket.PacketData"/> or to create new copies.
    /// </param>
    /// <param name="packetBytes">The raw byte array containing the packet data.</param>
    /// <param name="offset">The starting position in the byte array to begin parsing.</param>
    /// <returns>An <see cref="IpPacket"/> object containing the parsed packet data.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="packetBytes"/> is null.</exception>
    /// <exception cref="NotSupportedException">Thrown if the IP version is not IPv4.</exception>
    public static IpPacket Parse(DateTimeOffset receivedTime, bool reuseBuffer, byte[]? packetBytes, int offset)
    {
        if(packetBytes is null)
        {
            throw new ArgumentNullException(nameof(packetBytes), "Packet bytes cannot be null.");
        }

        var rawPacket = new RawIpPacket(new(packetBytes, offset, packetBytes.Length - offset));
        if(rawPacket.PayloadPacketOrData.Value.AsT0 is IpPacket ipPacket)
        {

            return ipPacket;
        }

        throw new InvalidOperationException("Invalid packet data");
    }
}
