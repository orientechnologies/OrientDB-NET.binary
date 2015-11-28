using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlDeleteEdgeTests
    {
        [Fact]
        public void ShouldDeleteEdgeFromDocumentOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestVertexClass")
                        .Extends<OVertex>()
                        .Run();

                    database
                        .Create.Class("TestEdgeClass")
                        .Extends<OEdge>()
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

                    Assert.Equal(1, documentsDeleted);
                }
            }
        }

        [Fact]
        public void ShouldDeleteEdgeFromObjectOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class<TestProfileClass>()
                        .Extends<OEdge>()
                        .Run();

                    ODocument vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OVertex>()
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

                    Assert.Equal(1, documentsDeleted);
                }
            }
        }

        [Fact]
        public void ShouldDeleteEdgeFromClassWhere()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    ODocument vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument edge1 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    int documentsDeleted = database
                        .Delete.Edge()
                        .Class<OEdge>()
                        .Where("bar").Equals(54321)
                        .Run();

                    Assert.Equal(1, documentsDeleted);
                }
            }
        }

        [Fact]
        public void ShouldDeleteEdgeFromDocumentToDocument()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    ODocument vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument edge1 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument edge2 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("foo", "foo string value2")
                        .Set("bar", 12345)
                        .Run();

                    string s = database
                        .Delete.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .ToString();

                    int documentsDeleted = database
                        .Delete.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Run();

                    Assert.Equal(2, documentsDeleted);
                }
            }
        }
    }
}
