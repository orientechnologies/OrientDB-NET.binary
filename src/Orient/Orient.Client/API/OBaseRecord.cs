using Orient.Client;

namespace Orient.Client
{
    public abstract class OBaseRecord
    {
        [OProperty(Serializable = false, Deserializable = false)]
        public ORID ORID { get; set; }

        [OProperty(Serializable = false, Deserializable = false)]
        public int OVersion { get; set; }

        [OProperty(Serializable = false, Deserializable = false)]
        public short OClassId { get; set; }

        [OProperty(Serializable = false, Deserializable = false)]
        public string OClassName { get; set; }

        public ODocument ToDocument()
        {
            return ODocument.ToDocument(this);
        }
    }
}
