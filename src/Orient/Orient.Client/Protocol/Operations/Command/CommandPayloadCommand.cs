using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations.Command
{
    internal class CommandPayloadCommand : CommandPayloadBase
    {
        public CommandPayloadCommand()
        {
            ClassName = "c";
        }
        internal byte[] SimpleParams { get; set; }
        internal byte[] ComplexParams { get; set; }
        internal new int PayLoadLength
        {
            get
            {
                // TODO: Implement Simple Complex params               
                return base.PayLoadLength
                    + (SimpleParams != null ? SimpleParams.Length : 0)
                    + (ComplexParams != null ? ComplexParams.Length : 0)
                    + sizeof(int) + BinarySerializer.Length(ClassName)
                    + sizeof(byte) + (SimpleParams != null ? SimpleParams.Length : 0)  // Has SimpleParams 0 - false , 1 - true
                    + sizeof(byte) + (ComplexParams != null ? ComplexParams.Length : 0); // Has ComplexParams 0 - false , 1 - true
            }
        }
    }
}
