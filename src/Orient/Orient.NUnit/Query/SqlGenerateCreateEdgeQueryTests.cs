using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class SqlGenerateCreateEdgeQueryTests
    {
        [Test]
        public void ShouldGenerateCreateEdgeClusterFromToQuery()
        {
            string generatedQuery = new OSqlCreateEdge()
                .Edge("TestEdgeClass")
                .Cluster("TestCluster")
                .From(new ORID(8, 0))
                .To(new ORID(8, 1))
                .ToString();

            string query =
                "CREATE EDGE TestEdgeClass " +
                "CLUSTER TestCluster " +
                "FROM #8:0 TO #8:1";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateEdgeFromToSetQuery()
        {
            string generatedQuery = new OSqlCreateEdge()
                .Edge("TestEdgeClass")
                .From(new ORID(8, 0))
                .To(new ORID(8, 1))
                .Set("foo", "foo string value")
                .Set("bar", 12345)
                .ToString();

            string query =
                "CREATE EDGE TestEdgeClass " +
                "FROM #8:0 TO #8:1 " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateEdgeObjectFromDocumentToDocumentQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.Name = "Johny";
            profile.Surname = "Bravo";

            ODocument vertexFrom = new ODocument();
            vertexFrom.ORID = new ORID(8, 0);

            ODocument vertexTo = new ODocument();
            vertexTo.ORID = new ORID(8, 1);

            string generatedQuery = new OSqlCreateEdge()
                .Edge(profile)
                .From(vertexFrom)
                .To(vertexTo)
                .ToString();

            string query =
                "CREATE EDGE TestProfileClass " +
                "FROM #8:0 TO #8:1 " +
                "SET Name = 'Johny', " +
                "Surname = 'Bravo'";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateEdgeObjectFromObjectToObjectQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.Name = "Johny";
            profile.Surname = "Bravo";

            TestProfileClass vertexFrom = new TestProfileClass();
            vertexFrom.ORID = new ORID(8, 0);

            TestProfileClass vertexTo = new TestProfileClass();
            vertexTo.ORID = new ORID(8, 1);

            string generatedQuery = new OSqlCreateEdge()
                .Edge(profile)
                .From(vertexFrom)
                .To(vertexTo)
                .ToString();

            string query =
                "CREATE EDGE TestProfileClass " +
                "FROM #8:0 TO #8:1 " +
                "SET Name = 'Johny', " +
                "Surname = 'Bravo'";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
