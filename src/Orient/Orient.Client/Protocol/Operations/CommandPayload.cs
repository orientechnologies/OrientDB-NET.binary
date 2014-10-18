
using Orient.Client.Protocol.Serializers;
namespace Orient.Client.Protocol.Operations
{
    internal class CommandPayloadQuery : CommandPayloadBase
    {
        public CommandPayloadQuery()
        {
            ClassName = "q";
        }
        internal int NonTextLimit { get; set; }
        internal string FetchPlan { get; set; }
        internal byte[] SerializedParams { get; set; }
        internal new int PayLoadLength
        {
            get
            {
                return base.PayLoadLength
                    + sizeof(int) + BinarySerializer.Length(ClassName)
                    + sizeof(int) // NonTextLimit
                    + sizeof(int) + BinarySerializer.Length(FetchPlan)
                    + sizeof(int) + (SerializedParams != null ? SerializedParams.Length : 0);
            }
        }
    }
    internal class CommandPayloadCommand : CommandPayloadBase
    {
        public CommandPayloadCommand()
        {
            ClassName = "c";
        }
        internal byte[] SimpleParams { get; set; }
        internal byte[] ComplexParams { get; set; }
        internal new int PayLoadLength
        {
            get
            {
                // TODO: Implement Simple Complex params               
                return base.PayLoadLength
                    + (SimpleParams != null ? SimpleParams.Length : 0)
                    + (ComplexParams != null ? ComplexParams.Length : 0)
                    + sizeof(int) + BinarySerializer.Length(ClassName)
                    + sizeof(byte) + (SimpleParams != null ? SimpleParams.Length : 0)  // Has SimpleParams 0 - false , 1 - true
                    + sizeof(byte) + (ComplexParams != null ? ComplexParams.Length : 0); // Has ComplexParams 0 - false , 1 - true
            }
        }
    }
    internal class CommandPayloadScript : CommandPayloadCommand
    {
        public CommandPayloadScript()
        {
            ClassName = "s";
        }
        private string _language;
        internal string Language
        {
            get { return _language; }
            set
            {
                if (value.ToLowerInvariant() == "gremlin")
                {
                    ClassName = "com.orientechnologies.orient.graph.gremlin.OCommandGremlin";
                }
                else
                {
                    ClassName = "s";
                }
                _language = value;
            }
        }
        internal new int PayLoadLength
        {
            get
            {
                return base.PayLoadLength
                    + sizeof(int) + BinarySerializer.Length(Language);
            }
        }

    }
    internal abstract class CommandPayloadBase
    {
        public virtual string ClassName { get; protected set; }
        internal string Text { get; set; }
        internal int PayLoadLength
        {
            get
            {
                return sizeof(int) + BinarySerializer.Length(Text);
            }
        }
    }
}
