using System.Net;

namespace SyslogSharp.Networking;
internal sealed record IpV4Packet : IpPacket
{
    /// <summary>
    /// Represents the minimum length, in bytes, required for a valid header.
    /// </summary>
    /// <remarks>This constant is used to validate that a header meets the minimum size requirements before
    /// processing. Headers shorter than this length are considered invalid.</remarks>
    private const int MinimumHeaderLength = 20;
    private readonly ushort _totalLength;

    public IpV4Packet(ArraySegment<byte> packetData, Packet? parent = default, DateTimeOffset receivedTime = default)
        : base(parent, receivedTime)
    {
        var ipVersion = (byte)(packetData[0] >> 4);
        if (ipVersion != 4)
            throw new NotSupportedException($"IP version {ipVersion} is not supported.");

        receivedTime = receivedTime == default ? DateTimeOffset.UtcNow : receivedTime;
        ReceivedAt = receivedTime;

        var headerLength = packetData[0] & 0x0F; // Internet Header Length in 32-bit words
        var headerLengthBytes = headerLength * 4; // 4 bytes per word
        Header = packetData.Slice(0, headerLengthBytes);

        _totalLength = (ushort)((packetData[2] << 8) | packetData[3]);

        if (headerLengthBytes < MinimumHeaderLength || packetData.Count < headerLengthBytes)
            throw new ArgumentOutOfRangeException(nameof(packetData), "Header length is less than 20 bytes.");

        
        PayloadPacketOrData = new(() =>
        {
            var payload = packetData.Slice(headerLengthBytes, TotalLength - headerLengthBytes);
            return ParsePayload(payload, Protocol, this);
        });

        DSCP = (byte)(packetData[1] >> 2);
        ECN = (byte)(packetData[1] & 0x03);
        Identification = (ushort)((packetData[4] << 8) | packetData[5]);
        var headerFlags = (ushort)((packetData[6] << 8) | packetData[7]);
        DF = (headerFlags & 0x4000) != 0;
        MF = (headerFlags & 0x2000) != 0;
        FragmentOffset = (ushort)(headerFlags & 0x1FFF);
        TTL = packetData[8];
        Protocol = (ProtocolType)packetData[9];
        HeaderChecksum = (ushort)((packetData[10] << 8) | packetData[11]);
        _sourceAddress = new IPAddress(packetData.Slice(12, 4));
        _destinationAddress = new IPAddress(packetData.Slice(16, 4));

        Options = [];
        if (headerLengthBytes > 20)
        {
            var length = headerLengthBytes - 20;
            Options = packetData.Slice(20, length);
        }
    }

    public override IPAddress SourceAddress => _sourceAddress;
    public override IPAddress DestinationAddress => _destinationAddress;
    protected override ushort TotalLength => _totalLength;

    /// <summary>
    /// Gets or sets the IP version
    /// </summary>
    public RawIpPacketProtocol IpProtocol { get; } = RawIpPacketProtocol.IpV4;

    /// <summary>
    /// Gets or sets the differentiated services code point.
    /// </summary>
    public byte DSCP { get; }

    /// <summary>
    /// Gets or sets the explicit congestion notification.
    /// </summary>
    public byte ECN { get; }

    /// <summary>
    /// Gets or sets the identification field, which is used for fragment reassembly.
    /// </summary>
    public ushort Identification { get; }

    /// <summary>
    /// Gets or sets the don't fragment flag
    /// </summary>
    public bool DF { get; }

    /// <summary>
    /// Gets or sets the more fragments flag
    /// </summary>
    public bool MF { get; }

    /// <summary>
    /// Gets or sets the fragment offset, which indicates the position of the fragment in the original packet.
    /// </summary>
    public ushort FragmentOffset { get; }

    /// <summary>
    /// Gets or sets the packet time to live (TTL), which specifies the maximum number of hops the packet can take before being discarded.
    /// </summary>
    public byte TTL { get; }

    /// <summary>
    /// Gets or sets the header checksum, which is used for error-checking the header.
    /// </summary>
    public ushort HeaderChecksum { get; }

    private readonly IPAddress _sourceAddress;
    private readonly IPAddress _destinationAddress;

    /// <summary>
    /// Gets or sets the IP options field, which may contain additional information about the packet.
    /// </summary>
    public ArraySegment<byte> Options { get; set; }
}
