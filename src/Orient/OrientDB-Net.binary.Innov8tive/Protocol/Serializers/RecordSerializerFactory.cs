using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.API.Types;

namespace Orient.Client.Protocol.Serializers
{
    public static class RecordSerializerFactory
    {
        public static IRecordSerializer GetSerializer(ODatabase database)
        {
            //if (database.ProtocolVersion < 22)
            //    return (IRecordSerializer)new RecordCSVSerializer(database.GetConnection());
            //else
            //    return (IRecordSerializer)new RecordBinarySerializer(database.GetConnection());

            // Temporary return old serializer
            // TODO: Fix after implement binary serializer compleatly

            return (IRecordSerializer)new RecordCSVSerializer((database != null) ? database.GetConnection() : null);
        }
        public static IRecordSerializer GetSerializer(ORecordFormat format)
        {
            switch (format)
            {
                case ORecordFormat.ORecordDocument2csv:
                    return new RecordCSVSerializer(null);

                case ORecordFormat.ORecordSerializerBinary:
                    return new RecordBinarySerializer(null);
            }
            throw new NotImplementedException("The " + format + " serializer not implemented it");
        }
    }
}
