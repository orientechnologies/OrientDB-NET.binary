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
                    + sizeof(byte) // Has SimpleParams 0 - false , 1 - true
                    + (SimpleParams != null ? sizeof(int) + SimpleParams.Length : 0)
                    + sizeof(byte) // Has ComplexParams 0 - false , 1 - true
                    + (ComplexParams != null ? sizeof(int) + ComplexParams.Length : 0);
            }
        }
    }
}
