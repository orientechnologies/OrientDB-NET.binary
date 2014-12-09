using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlDeleteClusterTest
    {
        [TestMethod]
        public void ShouldDeleteCluster()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var clusterid = database.Create.Cluster("TestClaster", OClusterType.Memory).Run();
                var clusters = database.GetClusters();
                var clusterLength = clusters.Count;

                Assert.IsTrue(clusterid > 0);
                Assert.IsTrue(clusters.Any(c => c.Id == clusterid));

                database.Delete.Cluster(clusterid).Run();
                Assert.AreEqual(clusterLength - 1, clusters.Count);
            }
        }
    }
}
