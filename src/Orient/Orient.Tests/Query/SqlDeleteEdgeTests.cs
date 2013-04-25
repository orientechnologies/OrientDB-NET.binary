using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlDeleteEdgeTests
    {
        [TestMethod]
        public void ShouldDeleteFromDocumentOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestVertexClass")
                        .Extends<OGraphVertex>()
                        .Run();

                    database
                        .Create.Class("TestEdgeClass")
                        .Extends<OGraphEdge>()
                        .Run();

                    ODocument vertex1 = database
                        .Create.Vertex("TestVertexClass")
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex("TestVertexClass")
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument edge1 = database
                        .Create.Edge("TestEdgeClass")
                        .From(vertex1)
                        .To(vertex2)
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    int documentsDeleted = database
                        .Delete.Edge(edge1)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [TestMethod]
        public void ShouldDeleteFromObjectOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class<TestProfileClass>()
                        .Extends<OGraphEdge>()
                        .Run();

                    ODocument vertex1 = database
                        .Create.Vertex<OGraphVertex>()
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OGraphVertex>()
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument edge1 = database
                        .Create.Edge<TestProfileClass>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("Name", "Johny")
                        .Set("Surnam", "Bravo")
                        .Run();

                    int documentsDeleted = database
                        .Delete.Edge(edge1)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [TestMethod]
        public void ShouldDeleteFromClassWhere()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    ODocument vertex1 = database
                        .Create.Vertex<OGraphVertex>()
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OGraphVertex>()
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument edge1 = database
                        .Create.Edge<OGraphEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    int documentsDeleted = database
                        .Delete.Edge()
                        .Class<OGraphEdge>()
                        .Where("bar").Equals(54321)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        /*[TestMethod]
        public void ShouldDeleteFromDocumentToDocument()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    ODocument vertex1 = database
                        .Create.Vertex<OGraphVertex>()
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OGraphVertex>()
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument edge1 = database
                        .Create.Edge<OGraphEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument edge2 = database
                        .Create.Edge<OGraphEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("foo", "foo string value2")
                        .Set("bar", 12345)
                        .Run();

                    int documentsDeleted = database
                        .Delete.Edge<OGraphEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 2);
                }
            }
        }*/
    }
}
