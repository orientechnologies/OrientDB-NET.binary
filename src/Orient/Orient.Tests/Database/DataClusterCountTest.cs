using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Database
{
    [TestClass]
    public class DataClusterCountTest
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
    }
}
