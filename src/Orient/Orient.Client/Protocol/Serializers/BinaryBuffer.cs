using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Serializers
{
    internal class BinaryBuffer : List<Byte>
    {
        public int Length { get { return Count; } }

        internal int Allocate(int allocationSize)
        {
            var currentPosition = Length;
            AddRange(new Byte[allocationSize]);
            return currentPosition;
        }

        internal void Write(int offset, int value)
        {
            var rawValue = BinarySerializer.ToArray(value);
            for (int i = 0; i < rawValue.Length; i++)
            {
                this[offset + i] = rawValue[i];
            }
        }

        internal void Write(int offset, byte value)
        {
            this[offset] = value;
        }

        internal int WriteVariant(long value)
        {
            var pos = Length;
            var unsignedValue = signedToUnsigned(value);
            writeUnsignedVarLong(unsignedValue);
            return pos;
        }

        private void writeUnsignedVarLong(ulong value)
        {
            while ((value & 0xFFFFFFFFFFFFFF80L) != 0L)
            {
                Add((byte)(value & 0x7F | 0x80));
                value >>= 7;
            }
            Add((byte)(value & 0x7F));
        }

        private uint signedToUnsigned(int value)
        {
            return (uint)((value << 1) ^ (value >> 31));
        }

        private ulong signedToUnsigned(long value)
        {
            return (ulong)((value << 1) ^ (value >> 63));
        }

        internal int Write(string value)
        {
            var pos = Length;
            WriteVariant(value.Length);
            AddRange(BinarySerializer.ToArray(value));
            return pos;
        }

        internal int Write(double value)
        {
            var pos = Length;
            AddRange(BitConverter.GetBytes(value).CheckEndianess());
            return pos;
        }

        internal int Write(float value)
        {
            var pos = Length;
            AddRange(BitConverter.GetBytes(value).CheckEndianess());
            return pos;
        }

        internal int Write(byte value)
        {
            var pos = Length;
            Add(value);
            return pos;
        }

        internal int Write(ORID rid)
        {
            var pos = Length;
            WriteVariant(rid.ClusterId);
            WriteVariant(rid.ClusterPosition);
            return pos;
        }

    }
}
