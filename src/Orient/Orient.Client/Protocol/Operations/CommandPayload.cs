
namespace Orient.Client.Protocol.Operations
{
    internal class CommandPayload
    {
        internal CommandPayloadType Type { get; set; }
        internal string Language { get; set; }
        internal string Text { get; set; }
        internal int NonTextLimit { get; set; }
        internal string FetchPlan { get; set; }
        internal byte[] SerializedParams { get; set; }
    }
}
