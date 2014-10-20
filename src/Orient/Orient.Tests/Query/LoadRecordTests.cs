using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class LoadRecordTests
    {

        [TestMethod]
        public void TestLoadNoFetchPlan()
        {
            using (var testContext = new TestDatabaseContext())
            {
                using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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
                    Assert.AreEqual("TestClass", loaded.OClassName);
                    Assert.AreEqual(document.GetField<string>("foo"), loaded.GetField<string>("foo"));
                    Assert.AreEqual(document.GetField<int>("bar"), loaded.GetField<int>("bar"));
                    Assert.AreEqual(insertedDocument.ORID, loaded.ORID);
                }
            }
        }
        [TestMethod]
        public void TestLoadRawDataRecordType()
        {

            using (var testContext = new TestDatabaseContext())
            {
                using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    var config = database.Load.ORID(new ORID(0, 0)).Run();
                    Assert.IsInstanceOfType(config, typeof(ODocument));
                    Assert.IsTrue(config.GetField<byte[]>("RawBytes").Length > 0);
                }
            }
        }
        [TestMethod]
        public void TestLoadWithFetchPlanNoLinks()
        {
            using (var testContext = new TestDatabaseContext())
            {
                using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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
                    Assert.AreEqual("TestClass", loaded.OClassName);
                    Assert.AreEqual(document.GetField<string>("foo"), loaded.GetField<string>("foo"));
                    Assert.AreEqual(document.GetField<int>("bar"), loaded.GetField<int>("bar"));
                    Assert.AreEqual(insertedDocument.ORID, loaded.ORID);
                }
            }
        }

        [TestMethod]
        public void TestLoadWithFetchPlanWithLinks()
        {
            using (var testContext = new TestDatabaseContext())
            {
                using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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
                    Assert.AreEqual("TestClass", loaded.OClassName);
                    Assert.AreEqual(document.GetField<string>("foo"), loaded.GetField<string>("foo"));
                    Assert.AreEqual(document.GetField<int>("bar"), loaded.GetField<int>("bar"));
                    Assert.AreEqual(insertedDocument.ORID, loaded.ORID);
                }
            }
        }
    }
}
