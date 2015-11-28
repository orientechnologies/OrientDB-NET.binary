using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Issues
{
    public class GitHub_issue71
    {
        private string _metadataQuery = "select expand(properties) from (select expand(classes) from #0:1) where name='TestClass'";

        [Fact]
        [Trait("type", "issue")]
        public void ShouldCreateNonPrimitiveProperties()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (var db = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                if (!db.Schema.IsClassExist<TestClass>())
                {
                    db.Create.Class<TestClass>().CreateProperties<TestClass>().Run();

                    var document = db.Query(_metadataQuery);

                    var SomeListMetadata = document.Find(d => d.GetField<string>("name") == "SomeList");
                    validateMetadata(SomeListMetadata, OType.EmbeddedList);

                    var DictionaryMetadata = document.Find(d => d.GetField<string>("name") == "Dictionary");
                    validateMetadata(DictionaryMetadata, OType.EmbeddedMap);
                }
            }
        }

        private void validateMetadata(ODocument metadata, OType expectedType)
        {
            Assert.NotNull(metadata);
            Assert.Equal(expectedType, (OType)metadata.GetField<int>("type"));
        }
        
        public class TestClass
        {
            public TestClass()
            {

            }
            public List<string> SomeList { get; set; }
            public string SomeString { get; set; }
            public int? NullableInt { get; set; }
            public HashSet<string> Hashset { get; set; }
            public Dictionary<string, string> Dictionary { get; set; }
            public TestEmbeddedClass EmbeddedClass { get; set; }
        }

        public class TestEmbeddedClass
        {
            public string StringProperty { get; set; }
        }
    }
}
