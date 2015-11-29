using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlCreateClusterTests
    {
        [Fact]
        public void ShouldCreateCluster()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short clusterId1 = database
                        .Create.Cluster("TestCluster1", OClusterType.Physical)
                        .Run();

                    Assert.True(clusterId1 > 0);

                    short clusterId2 = database
                        .Create.Cluster<TestProfileClass>(OClusterType.Physical)
                        .Run();

                    Assert.Equal(clusterId1 + 1, clusterId2);
                }
            }
        }
    }
}
