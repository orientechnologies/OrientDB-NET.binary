using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orient.Client.Protocol.Operations
{
    internal class ConfigSet : IOperation
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public Request Request(Request request)
        {
            // standard request fields
            request.AddDataItem((byte)OperationType.CONFIG_SET);
            request.AddDataItem(request.SessionId);

            request.AddDataItem(Key);
            request.AddDataItem(Value);
            return request;

        }

        public ODocument Response(Response response)
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
