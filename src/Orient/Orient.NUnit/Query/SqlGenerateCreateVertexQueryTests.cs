using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class SqlGenerateCreateVertexQueryTests
    {
        [Test]
        public void ShouldGenerateCreateVertexQuery()
        {
            string generatedQuery = new OSqlCreateVertex()
                .Vertex("TestVertexClass")
                .ToString();

            string query =
                "CREATE VERTEX TestVertexClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateVertexFromDocumentQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestVertexClass";
            document
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlCreateVertex()
                .Vertex(document)
                .ToString();

            string query =
                "CREATE VERTEX TestVertexClass " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateVertexFromObjectQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.Name = "Johny";
            profile.Surname = "Bravo";

            string generatedQuery = new OSqlCreateVertex()
                .Vertex(profile)
                .ToString();

            string query =
                "CREATE VERTEX TestProfileClass " +
                "SET Name = 'Johny', " +
                "Surname = 'Bravo'";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateVertexClusterQuery()
        {
            string generatedQuery = new OSqlCreateVertex()
                .Vertex("TestVertexClass")
                .Cluster("TestCluster")
                .ToString();

            string query =
                "CREATE VERTEX TestVertexClass " +
                "CLUSTER TestCluster";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateVertexClusterSetQuery()
        {
            string generatedQuery = new OSqlCreateVertex()
                .Vertex("TestVertexClass")
                .Cluster("TestCluster")
                .Set("foo", "foo string value")
                .Set("bar", 12345)
                .ToString();

            string query =
                "CREATE VERTEX TestVertexClass " +
                "CLUSTER TestCluster " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateVertexClusterSetFromDocumentQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestVertexClass";
            document
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlCreateVertex()
                .Vertex("TestVertexClass")
                .Cluster("TestCluster")
                .Set(document)
                .ToString();

            string query =
                "CREATE VERTEX TestVertexClass " +
                "CLUSTER TestCluster " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
