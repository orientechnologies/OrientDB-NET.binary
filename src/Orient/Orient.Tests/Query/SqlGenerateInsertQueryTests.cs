using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateInsertQueryTests
    {
        [TestMethod]
        public void ShouldGenerateInsertIntoSet()
        {
            string generatedUntypedQuery = new OSqlInsert()
                .Into("TestVertexClass")
                .Set("Foo", "new string value")
                .Set("Bar", 54321)
                .ToString();

            TestVertexClass obj = new TestVertexClass();
            obj.Foo = "new string value";
            obj.Bar = 54321;

            string generatedTypedQuery = new OSqlInsert()
                .Into<TestVertexClass>()
                .Set(obj)
                .ToString();

            string query =
                "INSERT INTO TestVertexClass " +
                "SET Foo = 'new string value', " +
                "Bar = 54321";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateInsertIntoClusterSet()
        {
            string generatedUntypedQuery = new OSqlInsert()
                .Into("TestVertexClass")
                .Cluster("TestCluster")
                .Set("Foo", "new string value")
                .Set("Bar", 54321)
                .ToString();

            TestVertexClass obj = new TestVertexClass();
            obj.Foo = "new string value";
            obj.Bar = 54321;

            string generatedTypedQuery = new OSqlInsert()
                .Into<TestVertexClass>()
                .Cluster("TestCluster")
                .Set(obj)
                .ToString();

            string query =
                "INSERT INTO TestVertexClass " +
                "CLUSTER TestCluster " +
                "SET Foo = 'new string value', " +
                "Bar = 54321";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }
    }
}
