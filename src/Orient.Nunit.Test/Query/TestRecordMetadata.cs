using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class TestRecordMetadata
    {
        [Test]
        public void ShouldRetrieveRecordMetadata()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    database
                        .Create
                        .Class("TestClass")
                        .Run();

                    var document = new ODocument();
                    document.OClassName = "TestClass";
                    document.SetField("bar", 12345);
                    document.SetField("foo", "foo value 345");

                    var createdDocument = database
                        .Create
                        .Document(document)
                        .Run();

                    var metadata = database
                        .Metadata
                        .ORID(createdDocument.ORID)
                        .Run();
                    Assert.IsNotNull(metadata);
                    Assert.AreEqual(createdDocument.ORID, metadata.ORID);
                    Assert.AreEqual(createdDocument.OVersion, metadata.OVersion);

                }
            }
        }
    }
}
