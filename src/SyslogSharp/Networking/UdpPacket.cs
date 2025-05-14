namespace SyslogSharp.Networking;
internal sealed record UdpPacket : TransportPacket
{
    public UdpPacket(ArraySegment<byte> packetData)
    {
        Header = packetData.Slice(0, 8);
        PayloadPacketOrData = new(() =>
        {
            PacketOrSegment? result = default;
            var payload = packetData.Slice(8, packetData.Count - 8);
            if(CustomPayloadParser is not null && (result = CustomPayloadParser(payload, this)) != null)
            {
                return result;
            }

            return payload;
        });

        var packetDataArray = packetData.Array!;

        Checksum = ParserHelpers.ReadNetOrderUShort(packetDataArray, packetData.Offset + 6);
        SourcePort = BitConverter.ToUInt16(packetDataArray, packetData.Offset);
        DestinationPort = BitConverter.ToUInt16(packetDataArray, packetData.Offset + 2);
        _totalLength = BitConverter.ToUInt16(packetDataArray, packetData.Offset + 4);
    }

    public UdpPacket(ArraySegment<byte> packetData, Packet parent)
        : this(packetData)
    {
        Parent = parent;
    }
    public override ushort Checksum { get; protected set; }
    public override ushort DestinationPort { get; protected set; }

    private readonly ushort _totalLength;

    public override ushort SourcePort { get; protected set; }

    protected override ushort TotalLength => _totalLength;
}
