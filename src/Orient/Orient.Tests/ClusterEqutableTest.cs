using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests
{
    [TestClass]
    public class ClusterEqutableTest
    {
        [TestMethod]
        public void ShoulBeEqualsClusters()
        {
            OCluster cluster1 = new OCluster { Id = 11, Name = "TestCluster", Type = OClusterType.Memory };
            OCluster cluster2 = new OCluster { Id = 11, Name = "TestCluster", Type = OClusterType.Memory };
            Assert.AreEqual(cluster1, cluster2);
            Assert.IsTrue(cluster1 == cluster2);
        }

        [TestMethod]
        public void ShoulNotBeEqualsClusters()
        {
            OCluster cluster1 = new OCluster { Id = 11, Name = "TestCluster" };
            OCluster cluster2 = new OCluster { Id = 11, Name = "TestCluster", Type = OClusterType.Memory };
            Assert.AreNotEqual(cluster1, cluster2);
            Assert.IsFalse(cluster1 == cluster2);
        }
    }
}
