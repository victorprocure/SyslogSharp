using System.Net;

namespace SyslogSharp.Networking;
internal abstract record IpPacket : Packet
{
    protected IpPacket(Packet? parent = default, DateTimeOffset receivedTime = default)
        : base(parent, receivedTime)
    {
        
    }
    /// <summary>
    /// Gets or sets the source IP address of the packet.
    /// </summary>
    public abstract IPAddress SourceAddress { get; }

    /// <summary>
    /// Gets or sets the destination IP address of the packet.
    /// </summary>
    public abstract IPAddress DestinationAddress { get; }


    /// <summary>
    /// Gets or sets the protocol type used for payload
    /// </summary>
    protected internal ProtocolType Protocol { get; protected set; }

    protected static PacketOrSegment ParsePayload(ArraySegment<byte> payload, ProtocolType protocol, Packet parent)
    {
        if(parent is IpV4Packet ipV4Packet)
        {
            if(ipV4Packet.FragmentOffset > 0)
            {
                return new(payload);
            }
        }else if(parent is IpV6Packet ipV6Packet)
        {
            if(ipV6Packet.ExtensionHeaders.Count > 0)
            {
                // TODO: Handle extension headers
                return new(payload);
            }
        }

            return protocol switch
            {
                ProtocolType.UDP => new UdpPacket(payload, parent),
                ProtocolType.IPv4 => new IpV4Packet(payload, parent),
                ProtocolType.IPv6 => new IpV6Packet(payload, parent),
                _ => payload
            };
    }
}
