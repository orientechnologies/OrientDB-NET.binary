
namespace Orient.Client.Protocol.Operations
{
    internal interface IOperation
    {
        Request Request(int sessionID);
        ODocument Response(Response response);
    }
}
