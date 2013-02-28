
namespace Orient.Client.Protocol
{
    internal interface IOperation
    {
        Request Request(int sessionID);
        object Response(Response response);
    }
}
