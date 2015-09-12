using Orient.Client;

namespace Orient.Tests
{
    public class TestProfileClass : OBaseRecord
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string[] StringArray { get; set; }
    }
}
