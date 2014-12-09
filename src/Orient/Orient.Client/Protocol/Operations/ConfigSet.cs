using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations
{
    internal class ConfigSet : BaseOperation
    {
        public ConfigSet(ODatabase database)
            : base(database)
        {

        }
        public string Key { get; set; }
        public string Value { get; set; }

        public override Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.CONFIG_SET);
            request.AddDataItem(request.SessionId);

            request.AddDataItem(Key);
            request.AddDataItem(Value);
            return request;

        }

        public override ODocument Response(Response response)
        {
            ODocument document = new ODocument();

            if (response == null)
            {
                return document;
            }

            if (response.Status == ResponseStatus.OK)
            {
                document.SetField("IsCreated", true);
            }
            else
            {
                document.SetField("IsCreated", true);
            }

            return document;
        }

    }
}
