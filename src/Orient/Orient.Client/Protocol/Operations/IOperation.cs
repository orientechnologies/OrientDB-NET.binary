
namespace Orient.Client.Protocol.Operations
{
    internal interface IOperation
    {
        Request Request(int sessionID);
        ResponseDataObject Response(Response response);
    }
}
