using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
    [TestFixture]
    public class LoadRecordTests
    {

        [Test]
        public void TestLoadNoFetchPlan()
        {
            using (var testContext = new TestDatabaseContext())
            {
                using (var database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document = new ODocument()
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    ODocument insertedDocument = database
                        .Insert(document)
                        .Into("TestClass")
                        .Run();
                    var loaded = database.Load.ORID(insertedDocument.ORID).Run();
                    Assert.AreEqual(loaded.OClassName, "TestClass");
                    Assert.AreEqual(loaded.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(loaded.GetField<int>("bar"), document.GetField<int>("bar"));
                    Assert.AreEqual(insertedDocument.ORID, loaded.ORID);
                }
            }
        }
        [Test]
        public void TestLoadRawDataRecordType()
        {

            using (var testContext = new TestDatabaseContext())
            {
                using (var database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    var config = database.Load.ORID(new ORID(0, 0)).Run();
                    
                    Assert.IsTrue(config.GetField<byte[]>("RawBytes").Length > 0);
                }
            }
        }
        [Test]
        public void TestLoadWithFetchPlanNoLinks()
        {
            using (var testContext = new TestDatabaseContext())
            {
                using (var database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document = new ODocument()
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    ODocument insertedDocument = database
                        .Insert(document)
                        .Into("TestClass")
                        .Run();
                    var loaded = database.Load.ORID(insertedDocument.ORID).FetchPlan("*:1").Run();
                    Assert.AreEqual(loaded.OClassName, "TestClass");
                    Assert.AreEqual(loaded.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(loaded.GetField<int>("bar"), document.GetField<int>("bar"));
                    Assert.AreEqual(insertedDocument.ORID, loaded.ORID);
                }
            }
        }
        [Ignore("Thows exception source record is not a vertex")]
        [Test]
        public void TestLoadWithFetchPlanWithLinks()
        {
            using (var testContext = new TestDatabaseContext())
            {
                using (var database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document = new ODocument()
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    ODocument insertedDocument = database
                        .Insert(document)
                        .Into("TestClass")
                        .Run();

                    for (int i = 0; i < 2; i++)
                    {

                        ODocument document2 = new ODocument()
                            .SetField("foo", "bar string value")
                            .SetField("bar", 23456);

                        ODocument insertedDocument2 = database
                            .Insert(document2)
                            .Into("TestClass")
                            .Run();
                        database.Create.Edge("E").From(insertedDocument).To(insertedDocument2).Run();
                    }


                    var loaded = database.Load.ORID(insertedDocument.ORID).FetchPlan("*:1").Run();
                    Assert.AreEqual(loaded.OClassName, "TestClass");
                    Assert.AreEqual(loaded.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(loaded.GetField<int>("bar"), document.GetField<int>("bar"));
                    Assert.AreEqual(insertedDocument.ORID, loaded.ORID);
                }
            }
        }
    }
}
