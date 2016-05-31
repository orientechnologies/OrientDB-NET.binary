
using Orient.Client.Protocol.Serializers;
namespace Orient.Client.Protocol.Operations.Command
{
    internal abstract class CommandPayloadBase
    {
        public virtual string ClassName { get; protected set; }
        internal string Text { get; set; }
        internal int PayLoadLength
        {
            get
            {
                return sizeof(int) + BinarySerializer.Length(ClassName) +
                       sizeof(int) + BinarySerializer.Length(Text);
            }
        }
    }
}
