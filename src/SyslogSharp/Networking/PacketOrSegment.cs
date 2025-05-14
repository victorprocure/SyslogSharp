using OneOf;

namespace SyslogSharp.Networking;

[GenerateOneOf]
internal partial class PacketOrSegment : OneOfBase<Packet, ArraySegment<byte>>
{
}
