using System.Collections.Generic;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Serializers;

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

        public void AddDataItem(byte b)
        {
            DataItems.Add(new RequestDataItem() {Type="byte", Data = BinarySerializer.ToArray(b)});
        }
        public void AddDataItem(short s)
        {
            DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray(s) });
        }
        public void AddDataItem(int i)
        {
            DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray(i) });
        }
        public void AddDataItem(long l)
        {
            DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray(l) });
        }
        public void AddDataItem(string s)
        {
            DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(s) });
        }

    }
}
