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
            :base(database)
        {

        }
        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.CONFIG_LIST);
            request.AddDataItem(request.SessionId);
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
