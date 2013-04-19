using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateCreateQueryTests
    {
        [TestMethod]
        public void ShouldGenerateCreateClassQuery()
        {
            string generatedUntypedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .Extends("OGraphVertex")
                .ToString();

            string generatedTypedQuery =new OSqlCreateClass()
                .Class<TestVertexClass>()
                .Extends<OGraphVertex>()
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass " +
                "EXTENDS OGraphVertex";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateCreateClusterQuery()
        {
            string generatedUntypedQuery = new OSqlCreateCluster()
                .Cluster("TestVertexClass", OClusterType.Physical)
                .ToString();

            string generatedTypedQuery = new OSqlCreateCluster()
                .Cluster<TestVertexClass>(OClusterType.Physical)
                .ToString();

            string query =
                "CREATE CLUSTER TestVertexClass PHYSICAL";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateCreateEdgeQuery()
        {
            string generatedUntypedQuery = new OSqlCreateEdge()
                .Edge("TestEdgeClass")
                .Cluster("OGraphEdge")
                .From(new ORID(8, 0))
                .To(new ORID(8, 1))
                .Set("Foo", "foo string value")
                .Set("Bar", 12345)
                .ToString();

            TestEdgeClass testObj = new TestEdgeClass();
            testObj.Foo = "foo string value";
            testObj.Bar = 12345;

            string generatedTypedQuery = new OSqlCreateEdge()
                .Edge<TestEdgeClass>()
                .Cluster<OGraphEdge>()
                .From(new ORID(8, 0))
                .To(new ORID(8, 1))
                .Set(testObj)
                .ToString();

            string query =
                "CREATE EDGE TestEdgeClass " +
                "CLUSTER OGraphEdge " +
                "FROM #8:0 TO #8:1 " +
                "SET Foo = 'foo string value', " +
                "Bar = 12345";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateCreateEdgeFromToDocumentQuery()
        {
            ODocument vertex1 = new ODocument();
            vertex1.ORID = new ORID(8, 0);

            ODocument vertex2 = new ODocument();
            vertex2.ORID = new ORID(8, 1);

            TestEdgeClass testObj = new TestEdgeClass();
            testObj.Foo = "foo string value";
            testObj.Bar = 12345;

            string generatedQuery = new OSqlCreateEdge()
                .Edge<TestEdgeClass>()
                .Cluster<OGraphEdge>()
                .From(vertex1)
                .To(vertex2)
                .Set(testObj)
                .ToString();

            string query =
                "CREATE EDGE TestEdgeClass " +
                "CLUSTER OGraphEdge " +
                "FROM #8:0 TO #8:1 " +
                "SET Foo = 'foo string value', " +
                "Bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateCreateVertexQuery()
        {
            string generatedUntypedQuery = new OSqlCreateVertex()
                .Vertex("TestVertexClass")
                .Cluster("OGraphVertex")
                .Set("Foo", "foo string value")
                .Set("Bar", 12345)
                .ToString();

            TestVertexClass testObj = new TestVertexClass();
            testObj.Foo = "foo string value";
            testObj.Bar = 12345;

            string generatedTypedQuery = new OSqlCreateVertex()
                .Vertex<TestVertexClass>()
                .Cluster<OGraphVertex>()
                .Set(testObj)
                .ToString();

            string query =
                "CREATE VERTEX TestVertexClass " +
                "CLUSTER OGraphVertex " +
                "SET Foo = 'foo string value', " +
                "Bar = 12345";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateCreateVertexDocumentQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestVertexClass";
            document
                .SetField("Foo", "foo string value")
                .SetField("Bar", 12345);

            string generatedQuery = new OSqlCreateVertex()
                .Vertex(document)
                .ToString();

            string query =
                "CREATE VERTEX TestVertexClass " +
                "SET Foo = 'foo string value', " +
                "Bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
