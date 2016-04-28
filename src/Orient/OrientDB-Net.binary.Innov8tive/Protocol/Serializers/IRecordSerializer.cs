using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Serializers
{
    public interface IRecordSerializer
    {
        byte[] Serialize(ODocument document);
        ODocument Deserialize(byte[] rawRecord, ODocument document);
    }
}
