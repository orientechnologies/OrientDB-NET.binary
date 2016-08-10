using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlCreatePropertyTest
    {
        private string _metadataQuery = "select expand(properties) from (select expand(classes) from #0:1) where name='TestClass'";

        [Test]
        public void ShouldCreateProperty()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Extends<OVertex>()
                        .Run();
                }

                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // Basic Test
                    foreach (var item in Enum.GetNames(typeof(OType)))
                    {
                        database.Create
                            .Property("_" + item.ToLower(), (OType)Enum.Parse(typeof(OType), item))
                            .Class("TestClass")
                            .Run();
                    }

                    var document = database.Query(_metadataQuery);

                    foreach (var item in Enum.GetNames(typeof(OType)))
                    {
                        var metadata = document.Find(d => d.GetField<string>("name") == "_" + item.ToLower());
                        validateMetadata(metadata, (OType)Enum.Parse(typeof(OType), item));
                    }

                    // Complex Test
                    database
                        .Create
                        .Property("_embededlist_with_type", OType.EmbeddedList)
                        .LinkedType(OType.Integer)
                        .Class("TestClass")
                        .Run();

                    database
                        .Create
                        .Property("_embededlist_with_class", OType.EmbeddedList)
                        .LinkedClass("OUser")
                        .Class("TestClass")
                        .Run();

                    document = database.Query(_metadataQuery);

                    var elwtMetadata = document.Find(d => d.GetField<string>("name") == "_embededlist_with_type");
                    validateMetadata(elwtMetadata, OType.EmbeddedList);
                    Assert.AreEqual(OType.Integer, (OType)elwtMetadata.GetField<int>("linkedType"));

                    var elwcMetadata = document.Find(d => d.GetField<string>("name") == "_embededlist_with_class");
                    validateMetadata(elwtMetadata, OType.EmbeddedList);
                    Assert.AreEqual("OUser", elwcMetadata.GetField<string>("linkedClass"));

                }
            }
        }

        private void validateMetadata(ODocument metadata, OType expectedType)
        {
            Assert.IsNotNull(metadata);
            Assert.AreEqual(expectedType, (OType)metadata.GetField<int>("type"));
        }
    }
}
