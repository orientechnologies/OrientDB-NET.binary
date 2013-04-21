using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateCreateClassQueryTests
    {
        [TestMethod]
        public void ShouldGenerateCreateClassQuery()
        {
            string generatedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateCreateClassExtendsQuery()
        {
            string generatedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .Extends("TestSuperClass")
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass " +
                "EXTENDS TestSuperClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
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

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
