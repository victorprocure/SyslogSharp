using System.Net;

namespace SyslogSharp.Networking;
internal static class ParserHelpers
{

    public static ushort ReadNetOrderUShort(byte[] bytes, int bufferOffset)
    {
        if (bytes.Length - bufferOffset < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferOffset), "Buffer offset overflows size of byte array.");
        }

        return (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, bufferOffset));
    }

    public static ArraySegment<byte> AsByteArraySegment(IReadOnlyCollection<byte> source)
    {
        if (source is ArraySegment<byte> sourceSegment)
        {
            return sourceSegment;
        }

        if (source is byte[] bytes)
        {
            return new ArraySegment<byte>(bytes);
        }

        return new ArraySegment<byte>([..source]);
    }
}
