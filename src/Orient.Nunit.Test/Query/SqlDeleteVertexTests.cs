using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlDeleteVertexTests
    {
        [Test]
        public void ShouldDeleteVertexFromDocumentOrid()
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

                    int documentsDeleted = database
                        .Delete.Vertex(vertex2)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [Test]
        public void ShouldDeleteVertexFromObjectOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class<TestProfileClass>()
                        .Extends<OVertex>()
                        .Run();

                    TestProfileClass vertex1 = database
                        .Create.Vertex<TestProfileClass>()
                        .Set("Name", "Johny")
                        .Set("Surname", "Bravo")
                        .Run<TestProfileClass>();

                    TestProfileClass vertex2 = database
                        .Create.Vertex<TestProfileClass>()
                        .Set("Name", "Julia")
                        .Set("Surname", "Bravo")
                        .Run<TestProfileClass>();

                    int documentsDeleted = database
                        .Delete.Vertex(vertex2)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [Test]
        public void ShouldDeleteVertexFromClassWhere()
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

                    ODocument document = new ODocument();
                    document.OClassName = "TestVertexClass";

                    int documentsDeleted = database
                        .Delete.Vertex()
                        .Class("TestVertexClass")
                        .Where("bar").Equals(12345)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }
    }
}
