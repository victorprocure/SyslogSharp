using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace SyslogSharp;
internal static class TypeIdentifierHelpers
{
    private static readonly byte[] NamespaceBytes =
        [
            72,
            44,
            45,
            178,
            195,
            144,
            71,
            200,
            135,
            248,
            26,
            21,
            191,
            193,
            48,
            251
        ];

   
    public static string GetTypeIdentifier(this object instance)
    {
        ArgumentNullException.ThrowIfNull(instance);

        var type = instance.GetType();

        var manifestId = type.GetTypeIdentifier();

        return manifestId;
    }

    public static string GetTypeIdentifier(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        var bondMapAttribute = ((GuidAttribute[])type.GetTypeInfo().GetCustomAttributes(typeof(GuidAttribute), false))
            .FirstOrDefault();

        if (bondMapAttribute != null &&
            !string.IsNullOrEmpty(bondMapAttribute.Value))
        {
            return bondMapAttribute.Value;
        }

        return GenerateGuidFromName(type.Name.ToUpperInvariant()).ToString();
    }

    public static string GetTypeIdentifier<T>()
    {
        return typeof(T).GetTypeIdentifier();
    }

    public static Guid GenerateGuidFromName(string name)
    {
        byte[] array;
        array = Encoding.BigEndianUnicode.GetBytes(name);

        byte[] buffer = new byte[array.Length + NamespaceBytes.Length];

        Buffer.BlockCopy(NamespaceBytes, 0, buffer, 0, NamespaceBytes.Length);
        Buffer.BlockCopy(array, 0, buffer, NamespaceBytes.Length, array.Length);

        array = SHA1.HashData(buffer);

        Array.Resize(ref array, 16);
        array[7] = (byte)(array[7] & 15 | 80);
        return new Guid(array);
    }
}
