namespace SyslogSharp.Networking;
internal record RawIpPacket : Packet
{
    private readonly ushort _totalLength;

    public RawIpPacket(ArraySegment<byte> packetData, DateTimeOffset receivedTime = default)
    {
        var version = (ushort)(packetData[0] >> 4);
        IpProtocol = (RawIpPacketProtocol)version;
        PayloadPacketOrData = new(() =>
        {
            Packet packet = IpProtocol switch
            {
                RawIpPacketProtocol.IpV4 => new IpV4Packet(packetData, receivedTime: receivedTime),
                RawIpPacketProtocol.IpV6 => new IpV6Packet(packetData, receivedTime: receivedTime),
                _ => throw new InvalidOperationException($"Unsupported IP version: {version}")
            };

            return packet;
        });

        _totalLength = (ushort)packetData.Count;
    }

    protected override ushort TotalLength => _totalLength;

    public RawIpPacketProtocol IpProtocol { get; }
}
