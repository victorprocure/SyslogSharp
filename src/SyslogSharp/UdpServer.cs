using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

using Microsoft.Extensions.Logging;

using SyslogSharp.Networking;

namespace SyslogSharp;
internal sealed class UdpServer : UdpReceiver<SyslogMessage>
{
    private static readonly SyslogEventParser _syslogEventParser = new();
    public UdpServer(IPAddress address, ushort port) : this(new IPEndPoint(address, port)){ }
    public UdpServer(string address, ushort port) : this(new IPEndPoint(IPAddress.Parse(address), port)){ }
    public UdpServer(IPEndPoint endPoint) : this(endPoint as EndPoint) { }
    private UdpServer(EndPoint endPoint) : base(endPoint)
    {
    }

    public Task StartAsync(CancellationToken _)
    {
        Logger.LogInformation("Starting syslog UDP server...");

        Start();
        return Task.CompletedTask;
    }

    protected override bool TryParsePacket(IpPacket packet, [NotNullWhen(true)] out SyslogMessage? data)
    {
        data = null;
        if(ListenEndpoint is IPEndPoint ipEndpoint && ipEndpoint.Address != IPAddress.Any && !packet.PacketHeader.DestinationIpAddress.Equals(ipEndpoint.Address))
        {
            Logger.LogWarning("Received packet with destination IP {DestinationIp} but server is listening on {ListeningIp}", packet.PacketHeader.DestinationIpAddress, ipEndpoint.Address);
            return false;
        }

        var udpDatagram = ConvertToDatagram(packet, false);
        var isValid = udpDatagram.UdpDatagramHeader?.DestinationPort == (ListenEndpoint as IPEndPoint)?.Port;
        if(!isValid)
        {
            Logger.LogWarning("Received packet with destination port {DestinationPort} but server is listening on {ListeningPort}", udpDatagram.UdpDatagramHeader?.DestinationPort, (ListenEndpoint as IPEndPoint)?.Port);
            return false;
        }

        SyslogEvent syslog;
        try
        {
            syslog = _syslogEventParser.Parse(udpDatagram.UdpData, udpDatagram.ReceivedTime, udpDatagram.PacketHeader.SourceIpAddress.ToString());
        }
        catch
        {
            return false;
        }

        data = new()
        {
            OccurrenceTime = packet.ReceivedTime,
            ReceivedTime = packet.ReceivedTime,
            Payload = udpDatagram.UdpData.Array,
            PayloadInstance = syslog
        };

        return true;
    }

    private static UdpDatagram ConvertToDatagram(IpPacket ipPacket, bool reuseOriginalBuffer = true)
    {
        ArgumentNullException.ThrowIfNull(ipPacket);

        if (ipPacket.ProtocolType != Networking.ProtocolType.UDP)
        {
            throw new NotSupportedException("Only UDP packets are supported.");
        }

        if (ipPacket.PacketData.Count <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ipPacket), "IP Packet data not provided.");
        }

        var packetData = ParserHelpers.AsByteArraySegment(ipPacket.PacketData);
        var packetArray = packetData.Array ?? throw new InvalidOperationException("Packet data array is null.");

        UdpDatagramHeader? udpDatagramHeader = null;
        if (ipPacket.PacketData.Count > 8)
        {
            udpDatagramHeader = new UdpDatagramHeader(
              ParserHelpers.ReadNetOrderUShort(packetArray, packetData.Offset),
              ParserHelpers.ReadNetOrderUShort(packetArray, packetData.Offset + 2),
              ParserHelpers.ReadNetOrderUShort(packetArray, packetData.Offset + 4),
              ParserHelpers.ReadNetOrderUShort(packetArray, packetData.Offset + 6));
        }

        ArraySegment<byte> udpData = reuseOriginalBuffer
            ? new(packetArray[(packetData.Offset + 8)..])
            : new(packetArray[(packetData.Offset + 8)..].ToArray());

        var udpDatagram = new UdpDatagram
        {
            PacketHeader = ipPacket.PacketHeader,
            UdpDatagramHeader = udpDatagramHeader,
            UdpData = udpData,
            ReceivedTime = ipPacket.ReceivedTime
        };

        return udpDatagram;
    }
}
