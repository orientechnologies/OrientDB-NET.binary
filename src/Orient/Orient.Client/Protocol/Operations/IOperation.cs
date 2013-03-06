
namespace Orient.Client.Protocol.Operations
{
    internal interface IOperation
    {
        Request Request(int sessionID);
        DataObject Response(Response response);
    }
}
