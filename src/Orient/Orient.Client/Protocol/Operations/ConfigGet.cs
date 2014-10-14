using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class ConfigGet : IOperation
    {
        internal string ConfigKey { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Request();

            // standard request fields
            request.AddDataItem((byte)OperationType.CONFIG_GET);
            request.AddDataItem(sessionID);

            request.AddDataItem(ConfigKey);
            return request;
        }

        public ODocument Response(Response response)
        {
            ODocument document = new ODocument();

            if (response == null)
            {
                return document;
            }

            BinaryReader reader = response.Reader;
            string value = reader.ReadInt32PrefixedString();
            document.SetField(ConfigKey, value);
            return document;
        }
    }
}
