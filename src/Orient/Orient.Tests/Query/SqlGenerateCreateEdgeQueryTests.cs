using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateCreateEdgeQueryTests
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
        public void ShouldGenerateCreateEdgeDocumentFromVertexToVertexQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestEdgeClass";
            document
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            ODocument vertexFrom = new ODocument();
            vertexFrom.ORID = new ORID(8, 0);

            ODocument vertexTo = new ODocument();
            vertexTo.ORID = new ORID(8, 1);

            string generatedQuery = new OSqlCreateEdge()
                .Edge(document)
                .From(vertexFrom)
                .To(vertexTo)
                .ToString();

            string query =
                "CREATE EDGE TestEdgeClass " +
                "FROM #8:0 TO #8:1 " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
