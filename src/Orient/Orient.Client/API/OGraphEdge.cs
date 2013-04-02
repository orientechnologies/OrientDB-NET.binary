
namespace Orient.Client
{
    public class OGraphEdge
    {
        [OProperty(MappedTo = "in")]
        public ORID In { get; set; }

        [OProperty(MappedTo = "out")]
        public ORID Out { get; set; }
    }
}
