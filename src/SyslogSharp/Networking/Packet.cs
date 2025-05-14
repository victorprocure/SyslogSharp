namespace SyslogSharp.Networking;

internal abstract record Packet
{
    protected Packet(Packet? packet, DateTimeOffset receivedAt = default)
    {
        Parent = packet;
        ReceivedAt = receivedAt;
    }

    protected Packet()
    {
        
    }

    protected internal ArraySegment<byte> Header { get; protected set; }
    protected internal Packet? Parent { get; protected set; }
    protected internal Lazy<PacketOrSegment> PayloadPacketOrData { get; protected set; } = new();

    /// <summary>
    /// Gets or sets the total length of the IP packet, including the header and payload.
    /// </summary>
    protected abstract ushort TotalLength { get; }

    protected internal DateTimeOffset ReceivedAt { get; protected set; }
}
