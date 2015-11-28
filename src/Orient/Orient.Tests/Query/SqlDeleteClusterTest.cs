using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlDeleteClusterTest
    {
        [Fact]
        public void ShouldDeleteCluster()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var clusterid = database.Create.Cluster("TestClaster", OClusterType.Memory).Run();
                var clusters = database.GetClusters();
                var clusterLength = clusters.Count;

                Assert.True(clusterid > 0);
                Assert.True(clusters.Any(c => c.Id == clusterid));

                database.Delete.Cluster(clusterid).Run();
//                Assert.Equal(clusterLength - 1, clusters.Count);
            }
        }
    }
}
