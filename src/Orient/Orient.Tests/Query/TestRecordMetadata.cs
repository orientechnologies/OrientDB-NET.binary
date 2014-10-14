using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;
using Orient.Client.API.Query;

namespace Orient.Tests.Query
{
    [TestClass]
    public class TestRecordMetadata
    {
        [TestMethod]
        public void ShouldRetrieveRecordMetadata()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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
