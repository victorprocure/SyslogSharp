namespace SyslogSharp.Networking;

/// <summary>
/// Represents the protocol version used in a raw IP packet.
/// </summary>
/// <remarks>This enumeration defines the protocol version for raw IP packets,  such as IPv4 or IPv6, and is
/// typically used to specify the version  when working with low-level network operations.</remarks>
internal enum RawIpPacketProtocol : ushort
{
    /// <summary>
    /// Represents the Internet Protocol version 4 (IPv4).
    /// </summary>
    IpV4 = 4,

    /// <summary>
    /// Represents the Internet Protocol version 6 (IPv6).
    /// </summary>
    IpV6 = 6
}
