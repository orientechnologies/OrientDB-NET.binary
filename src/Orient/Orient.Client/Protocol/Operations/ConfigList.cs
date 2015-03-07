using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class ConfigList : BaseOperation
    {
        public ConfigList(ODatabase database)
            : base(database)
        {
            _operationType = OperationType.CONFIG_LIST;
        }
        public override Request Request(Request request)
        {
            base.Request(request);
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

            short len = reader.ReadInt16EndianAware();
            Dictionary<string, string> configList = new Dictionary<string, string>();
            for (int i = 0; i < len; i++)
            {
                string key = reader.ReadInt32PrefixedString();
                string value = reader.ReadInt32PrefixedString();
                configList.Add(key, value);
            }
            document.SetField<Dictionary<string, string>>("config", configList);
            return document;
        }

    }
}
