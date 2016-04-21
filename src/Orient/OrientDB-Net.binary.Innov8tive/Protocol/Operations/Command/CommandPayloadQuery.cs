using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations.Command
{
    internal class CommandPayloadQuery : CommandPayloadBase
    {
        public CommandPayloadQuery()
        {
            ClassName = "q";
        }
        internal int NonTextLimit { get; set; }
        internal string FetchPlan { get; set; }
        internal byte[] SerializedParams { get; set; }
        internal new int PayLoadLength
        {
            get
            {
                return base.PayLoadLength
                    + sizeof(int) // NonTextLimit
                    + sizeof(int) + BinarySerializer.Length(FetchPlan)
                    + sizeof(int) + (SerializedParams != null ? SerializedParams.Length : 0);
            }
        }
    }
}
