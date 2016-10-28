using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlGenerateCreateClusterQueryTests
    {
        [Test]
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
