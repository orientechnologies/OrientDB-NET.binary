using System.Collections.Generic;

namespace Orient.Client.Protocol
{
    internal class Request
    {
        internal OperationMode OperationMode { get; set; }
        internal List<DataItem> DataItems { get; set; }

        internal Request()
        {
            OperationMode = OperationMode.Synchronous;
            DataItems = new List<DataItem>();
        }
    }
}
