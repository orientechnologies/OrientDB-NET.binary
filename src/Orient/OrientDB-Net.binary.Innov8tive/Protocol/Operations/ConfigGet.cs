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
            _operationType = OperationType.CONFIG_GET;
        }
        internal string ConfigKey { get; set; }

        public override Request Request(Request request)
        {
            base.Request(request);

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
            
            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

            string value = reader.ReadInt32PrefixedString();
            document.SetField(ConfigKey, value);
            return document;
        }

    }
}
