using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Database
{
    [TestClass]
    public class DataClusterTest
    {
        [TestMethod]
        public void ShouldRetrieveNumberRecordsInSingleCluster()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Create.Class("TestDocumentClass").Run();

                for (int i = 0; i < 100; i++)
                {
                    database
                        .Create
                        .Document("TestDocumentClass")
                        .Set("bar", i)
                        .Run();
                }

                long recordsInCluster = database
                    .Clusters("TestDocumentClass")
                    .Count();

                Assert.AreEqual(100, recordsInCluster);
            }
        }

        [TestMethod]
        public void ShouldRetrieveDataRangeForSingle()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var document = database.Clusters("OUser").Range();
                Assert.IsNotNull(document);
                Assert.AreEqual(1, document.Count);
                Assert.IsTrue(document.ContainsKey("OUser"));
                
                var document1 = database.Clusters(4).Range();
                Assert.IsNotNull(document1);
                Assert.AreEqual(1, document1.Count);
                Assert.IsTrue(document1.ContainsKey("4"));
            }
        }

        [TestMethod]
        public void ShouldRetrieveDataRangeForMany()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var document = database.Clusters("OUser","ORole","V").Range();
                Assert.IsNotNull(document);
                Assert.AreEqual(3, document.Count);
                Assert.IsTrue(document.ContainsKey("OUser"));
                Assert.IsTrue(document.ContainsKey("ORole"));
                Assert.IsTrue(document.ContainsKey("V"));
                
                var document1 = database.Clusters(4, 5, 9).Range();
                Assert.IsNotNull(document1);
                Assert.AreEqual(3, document1.Count);
                Assert.IsTrue(document1.ContainsKey("4"));
                Assert.IsTrue(document1.ContainsKey("5"));
                Assert.IsTrue(document1.ContainsKey("9"));

            }
        }
    }
}
