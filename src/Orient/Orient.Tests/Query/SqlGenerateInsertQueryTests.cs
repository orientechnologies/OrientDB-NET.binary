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
                .Set("foo", "new string value")
                .Set("bar", 54321)
                .ToString();

            ODocument document = new ODocument();
            document.SetField("foo", "new string value");
            document.SetField("bar", 54321);

            string generatedTypedQuery = new OSqlInsert()
                .Into<TestVertexClass>()
                .Set(document)
                .ToString();

            string query =
                "INSERT INTO TestVertexClass " +
                "SET foo = 'new string value', " +
                "bar = 54321";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateInsertIntoClusterSet()
        {
            string generatedUntypedQuery = new OSqlInsert()
                .Into("TestVertexClass")
                .Cluster("TestCluster")
                .Set("foo", "new string value")
                .Set("bar", 54321)
                .ToString();

            ODocument document = new ODocument();
            document.SetField("foo", "new string value");
            document.SetField("bar", 54321);

            string generatedTypedQuery = new OSqlInsert()
                .Into<TestVertexClass>()
                .Cluster("TestCluster")
                .Set(document)
                .ToString();

            string query =
                "INSERT INTO TestVertexClass " +
                "CLUSTER TestCluster " +
                "SET foo = 'new string value', " +
                "bar = 54321";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }
    }
}
