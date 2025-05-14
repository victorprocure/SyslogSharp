namespace SyslogSharp.Networking;
internal abstract record TransportPacket : Packet
{
    internal static Func<ArraySegment<byte>, TransportPacket, PacketOrSegment>? CustomPayloadParser { get; }
    public abstract ushort Checksum { get; protected set; }
    public abstract ushort DestinationPort { get; protected set; }
    public abstract ushort SourcePort { get; protected set; }
}
