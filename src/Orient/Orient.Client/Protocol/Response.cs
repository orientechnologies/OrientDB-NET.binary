
namespace Orient.Client.Protocol
{
    internal class Response
    {
        internal ResponseStatus Status { get; set; }
        internal int SessionId { get; set; }
        internal byte[] Data { get; set; }
    }
}
