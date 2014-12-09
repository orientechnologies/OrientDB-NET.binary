
using Orient.Client.Protocol.Serializers;
namespace Orient.Client.Protocol.Operations
{
    internal interface IOperation
    {
        IRecordSerializer Serializer { get; }
        ODocument Response(Response response);
        Request Request(Request req);
    }
}
