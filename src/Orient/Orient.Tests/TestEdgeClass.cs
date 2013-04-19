using Orient.Client;

namespace Orient.Tests
{
    public class TestEdgeClass
    {
        [OProperty(Alias = "in", Serializable = false)]
        public ORID In { get; set; }

        [OProperty(Alias = "out", Serializable = false)]
        public ORID Out { get; set; }

        public string Foo { get; set; }
        public int Bar { get; set; }
    }
}
