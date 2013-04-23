
namespace Orient.Client
{
    public class OGraphEdge : OBaseRecord
    {
        [OProperty(Alias = "in")]
        public ORID In { get; set; }

        [OProperty(Alias = "out")]
        public ORID Out { get; set; }
    }
}
