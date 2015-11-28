using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests
{
    
    public class ClusterEqutableTest
    {
        [Fact]
        public void ShoulBeEqualsClusters()
        {
            OCluster cluster1 = new OCluster { Id = 11, Name = "TestCluster", Type = OClusterType.Memory };
            OCluster cluster2 = new OCluster { Id = 11, Name = "TestCluster", Type = OClusterType.Memory };
            Assert.Equal(cluster1, cluster2);
            Assert.True(cluster1 == cluster2);
        }

        [Fact]
        public void ShoulNotBeEqualsClusters()
        {
            OCluster cluster1 = new OCluster { Id = 11, Name = "TestCluster" };
            OCluster cluster2 = new OCluster { Id = 11, Name = "TestCluster", Type = OClusterType.Memory };
            Assert.NotEqual(cluster1, cluster2);
            Assert.False(cluster1 == cluster2);
        }
    }
}
