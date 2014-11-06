using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class ConfigGet : BaseOperation
    {
        public ConfigGet(ODatabase database)
            : base(database)
        {

        }
        internal string ConfigKey { get; set; }

        public override Request Request(Request request)
        {

            // standard request fields
            request.AddDataItem((byte)OperationType.CONFIG_GET);
            request.AddDataItem(request.SessionId);

            request.AddDataItem(ConfigKey);
            return request;
        }

        public override ODocument Response(Response response)
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
