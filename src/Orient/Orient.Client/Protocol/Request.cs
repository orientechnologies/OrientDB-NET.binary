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

        internal void AddDataItem(byte b)
        {
            DataItems.Add(new RequestDataItem() { Type = "byte", Data = BinarySerializer.ToArray(b) });
        }
        internal void AddDataItem(short s)
        {
            DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray(s) });
        }
        internal void AddDataItem(int i)
        {
            DataItems.Add(new RequestDataItem() { Type = "int", Data = BinarySerializer.ToArray(i) });
        }
        internal void AddDataItem(long l)
        {
            DataItems.Add(new RequestDataItem() { Type = "long", Data = BinarySerializer.ToArray(l) });
        }
        internal void AddDataItem(string s)
        {
            DataItems.Add(new RequestDataItem() { Type = "string", Data = BinarySerializer.ToArray(s) });
        }
        internal void AddDataItem(ORID _orid)
        {
            DataItems.Add(new RequestDataItem() { Type = "short", Data = BinarySerializer.ToArray(_orid.ClusterId) });
            DataItems.Add(new RequestDataItem() { Type = "long", Data = BinarySerializer.ToArray(_orid.ClusterPosition) });
        }

        internal void AddDataItem(byte[] value)
        {
            DataItems.Add(new RequestDataItem() { Type = "bytes", Data = value });
        }
    }
}
