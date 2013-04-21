using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateCreateClusterQueryTests
    {
        [TestMethod]
        public void ShouldGenerateCreateClusterQuery()
        {
            string generatedQuery = new OSqlCreateCluster()
                .Cluster("TestVertexClass", OClusterType.Physical)
                .ToString();

            string query =
                "CREATE CLUSTER TestVertexClass PHYSICAL";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
