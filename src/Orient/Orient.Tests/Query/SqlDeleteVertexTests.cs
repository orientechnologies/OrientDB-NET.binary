using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlDeleteVertexTests
    {
        [TestMethod]
        public void ShouldDeleteVertexFromDocumentOrid()
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

        [TestMethod]
        public void ShouldDeleteVertexFromObjectOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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

        [TestMethod]
        public void ShouldDeleteVertexFromClassWhere()
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
