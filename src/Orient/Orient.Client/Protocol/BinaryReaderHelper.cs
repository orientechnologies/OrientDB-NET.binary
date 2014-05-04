using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol
{
    /// <summary>
    /// Contains helper methods to read int16, int32 and int64 while being endian-aware.
    /// </summary>
    public static class BinaryReaderHelper
    {
        public static byte[] CheckEndianess(this byte[] b)
        {
            if (BitConverter.IsLittleEndian)
                Array.Reverse(b);
            return b;
        }

        public static UInt16 ReadUInt16EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt16(binRdr.ReadBytesRequired(sizeof(UInt16)).CheckEndianess(), 0);
        }

        public static Int16 ReadInt16EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToInt16(binRdr.ReadBytesRequired(sizeof(Int16)).CheckEndianess(), 0);
        }

        public static UInt32 ReadUInt32EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt32(binRdr.ReadBytesRequired(sizeof(UInt32)).CheckEndianess(), 0);
        }

        public static Int32 ReadInt32EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToInt32(binRdr.ReadBytesRequired(sizeof(Int32)).CheckEndianess(), 0);
        }

        public static UInt64 ReadUInt64EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToUInt64(binRdr.ReadBytesRequired(sizeof(UInt64)).CheckEndianess(), 0);
        }

        public static Int64 ReadInt64EndianAware(this BinaryReader binRdr)
        {
            return BitConverter.ToInt64(binRdr.ReadBytesRequired(sizeof(Int64)).CheckEndianess(), 0);
        }

        public static byte[] ReadBytesRequired(this BinaryReader binRdr, int byteCount)
        {
            var result = binRdr.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} bytes required from stream, but only {1} returned.", byteCount, result.Length));

            return result;
        }

        public static string ReadInt32PrefixedString(this BinaryReader binRdr)
        {
            int length = binRdr.ReadInt32EndianAware();

            if (length < 0)
              return null;

            return System.Text.Encoding.Default.GetString(binRdr.ReadBytes(length));
        }
    }
}
