using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlInsertTests
    {
        /*[TestMethod]
        public void ShouldInsertIntoSet()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document = new ODocument();
                    document.SetField("foo", "foo string value");
                    document.SetField("bar", 12345);

                    ODocument insertedDocument = database
                        .Insert.Into("TestClass")
                        .Set(document)
                        .Run();

                    Assert.AreEqual(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));
                }
            }
        }

        [TestMethod]
        public void ShouldInsertIntoClusterSet()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short clusterId = database
                        .Create.Cluster("TestCluster", OClusterType.Physical)
                        .Run();

                    database
                        .Create.Class("TestClass")
                        .Cluster(clusterId)
                        .Run();

                    ODocument document = new ODocument();
                    document.SetField("foo", "foo string value");
                    document.SetField("bar", 12345);

                    ODocument insertedDocument = database
                        .Insert.Into("TestClass")
                        .Cluster("TestCluster")
                        .Set(document)
                        .Run();

                    Assert.AreEqual(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));
                }
            }
        }*/
    }
}
