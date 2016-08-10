using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlDeleteEdgeTests
    {
        [Test]
        public void ShouldDeleteEdgeFromDocumentOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [Test]
        public void ShouldDeleteEdgeFromObjectOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [Test]
        public void ShouldDeleteEdgeFromClassWhere()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [Test]
        public void ShouldDeleteEdgeFromDocumentToDocument()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

                    Assert.AreEqual(documentsDeleted, 2);
                }
            }
        }
    }
}
