using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlGenerateCreateClusterQueryTests
    {
        [Fact]
        public void ShouldGenerateCreateClusterQuery()
        {
            string generatedQuery = new OSqlCreateCluster()
                .Cluster("TestVertexClass", OClusterType.Physical)
                .ToString();

            string query =
                "CREATE CLUSTER TestVertexClass PHYSICAL";

            Assert.Equal(generatedQuery, query);
        }
    }
}
