using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlGenerateCreateClassQueryTests
    {
        [Fact]
        public void ShouldGenerateCreateClassQuery()
        {
            string generatedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateCreateClassExtendsQuery()
        {
            string generatedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .Extends("TestSuperClass")
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass " +
                "EXTENDS TestSuperClass";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateCreateClassExtendsClusterQuery()
        {
            string generatedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .Extends("TestSuperClass")
                .Cluster(8)
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass " +
                "EXTENDS TestSuperClass " +
                "CLUSTER 8";

            Assert.Equal(generatedQuery, query);
        }
    }
}
