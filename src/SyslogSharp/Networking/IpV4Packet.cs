using System.Net.Sockets;
using System.Net;

namespace SyslogSharp.Networking;
internal record struct IpV4Packet
{
    /// <summary>
    /// Gets or sets the IP version
    /// </summary>
    public byte Version { get; set; }

    /// <summary>
    /// Gets or sets the Internet Header Length (IHL) in 32-bit words.
    /// </summary>
    public byte IHL { get; set; }

    /// <summary>
    /// Gets or sets the differentiated services code point.
    /// </summary>
    public byte DSCP { get; set; }

    /// <summary>
    /// Gets or sets the explicit congestion notification.
    /// </summary>
    public byte ECN { get; set; }

    /// <summary>
    /// Gets or sets the total length of the IP packet, including the header and payload.
    /// </summary>
    public ushort TotalLength { get; set; }

    /// <summary>
    /// Gets or sets the identification field, which is used for fragment reassembly.
    /// </summary>
    public ushort Identification { get; set; }

    /// <summary>
    /// Gets or sets the don't fragment flag
    /// </summary>
    public bool DF { get; set; }

    /// <summary>
    /// Gets or sets the more fragments flag
    /// </summary>
    public bool MF { get; set; }

    /// <summary>
    /// Gets or sets the fragment offset, which indicates the position of the fragment in the original packet.
    /// </summary>
    public ushort FragmentOffset { get; set; }

    /// <summary>
    /// Gets or sets the packet time to live (TTL), which specifies the maximum number of hops the packet can take before being discarded.
    /// </summary>
    public byte TTL { get; set; }

    /// <summary>
    /// Gets or sets the protocol type used for payload
    /// </summary>
    public ProtocolType Protocol { get; set; }

    /// <summary>
    /// Gets or sets the header checksum, which is used for error-checking the header.
    /// </summary>
    public ushort HeaderChecksum { get; set; }

    /// <summary>
    /// Gets or sets the source IP address of the packet.
    /// </summary>
    public IPAddress SourceAddress { get; set; }

    /// <summary>
    /// Gets or sets the destination IP address of the packet.
    /// </summary>
    public IPAddress DestinationAddress { get; set; }

    /// <summary>
    /// Gets or sets the IP options field, which may contain additional information about the packet.
    /// </summary>
    public ArraySegment<byte> Options { get; set; }

    /// <summary>
    /// Gets or sets the payload data of the packet.
    /// </summary>
    public ArraySegment<byte> Payload { get; set; }

    /// <summary>
    /// Gets or sets when the packet was received
    /// </summary>
    public DateTimeOffset ReceivedAt { get; set; }
    public ushort HeaderFlags { get; internal set; }
}
