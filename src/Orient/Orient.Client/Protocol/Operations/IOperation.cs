
namespace Orient.Client.Protocol.Operations
{
    internal interface IOperation
    {
        Request Request(int sessionID);
        ODataObject Response(Response response);
    }
}
