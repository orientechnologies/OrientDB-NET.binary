
namespace Orient.Client.Protocol.Operations
{
    internal interface IOperation
    {
        ODocument Response(Response response);
        Request Request(Request req);
    }
}
