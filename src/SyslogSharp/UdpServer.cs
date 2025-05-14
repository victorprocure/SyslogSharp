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

    protected override bool TryParsePacket(TransportPacket packet, [NotNullWhen(true)] out SyslogMessage? data)
    {
        data = null;
        if (packet.Parent is not IpPacket parent)
        {
            Logger.LogWarning("Received packet with no parent IP packet.");
            return false;
        }

        if (ListenEndpoint is IPEndPoint ipEndpoint && ipEndpoint.Address != IPAddress.Any && !parent.DestinationAddress.Equals(ipEndpoint.Address))
        {
            Logger.LogWarning("Received packet with destination IP {DestinationIp} but server is listening on {ListeningIp}", parent.DestinationAddress, ipEndpoint.Address);
            return false;
        }


        var isValid = packet.DestinationPort == (ListenEndpoint as IPEndPoint)?.Port;
        if(!isValid)
        {
            Logger.LogWarning("Received packet with destination port {DestinationPort} but server is listening on {ListeningPort}", packet.DestinationPort, (ListenEndpoint as IPEndPoint)?.Port);
            return false;
        }

        if(!packet.PayloadPacketOrData.Value.TryPickT1(out var udpData, out var _))
        {
            Logger.LogWarning("Received packet with invalid payload data.");
            return false;
        }

        SyslogEvent syslog;
        try
        {
            syslog = _syslogEventParser.Parse(udpData, packet.ReceivedAt, parent.SourceAddress.ToString());
        }
        catch
        {
            return false;
        }

        data = new()
        {
            OccurrenceTime = packet.ReceivedAt,
            ReceivedTime = packet.ReceivedAt,
            Payload = [.. udpData],
            PayloadInstance = syslog
        };

        return true;
    }
}
