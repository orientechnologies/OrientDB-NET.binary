using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Serializers
{
    internal class BinaryBuffer : List<Byte>
    {
        public int Length { get { return Count; } }

        public int Allocate(int allocationSize)
        {
            var currentPosition = Length;
            AddRange(new Byte[allocationSize]);
            return currentPosition;
        }
    }
}
