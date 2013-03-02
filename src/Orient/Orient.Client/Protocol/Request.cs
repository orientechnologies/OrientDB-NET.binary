using System.Collections.Generic;
using Orient.Client.Protocol.Operations;

namespace Orient.Client.Protocol
{
    internal class Request
    {
        internal OperationMode OperationMode { get; set; }
        internal List<RequestDataItem> DataItems { get; set; }

        internal Request()
        {
            OperationMode = OperationMode.Synchronous;
            DataItems = new List<RequestDataItem>();
        }
    }
}
