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
            _operationType = OperationType.CONFIG_SET;
        }
        public string Key { get; set; }
        public string Value { get; set; }

        public override Request Request(Request request)
        {
            base.Request(request);

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
            
            var reader = response.Reader;
            if (response.Connection.ProtocolVersion > 26 && response.Connection.UseTokenBasedSession)
                ReadToken(reader);

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
