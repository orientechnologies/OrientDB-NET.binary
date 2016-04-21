using Orient.Client.Protocol.Serializers;

namespace Orient.Client.Protocol.Operations.Command
{
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
                var res = base.PayLoadLength;
                return (Language == "gremlin") ? res : res + sizeof(int) + BinarySerializer.Length(Language);
            }
        }

    }
}
