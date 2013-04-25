using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlCreateEdgeTests
    {
        [TestMethod]
        public void ShouldCreateEdgeClusterFromTo()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestEdgeClass")
                        .Extends<OGraphEdge>()
                        .Run();

                    database
                        .Create.Cluster("TestCluster", OClusterType.Physical)
                        .Run();

                    ODocument vertex1 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run();

                    ODocument createdEdge = database
                        .Create.Edge("TestEdgeClass")
                        .Cluster("TestCluster")
                        .From(vertex1.ORID)
                        .To(vertex2.ORID)
                        .Run();

                    Assert.IsTrue(createdEdge.ORID != null);
                    Assert.AreEqual(createdEdge.GetField<ORID>("in"), vertex2.ORID);
                    Assert.AreEqual(createdEdge.GetField<ORID>("out"), vertex1.ORID);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateEdgeFromToSet()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestEdgeClass")
                        .Extends<OGraphEdge>()
                        .Run();

                    ODocument vertex1 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run();

                    ODocument createdEdge = database
                        .Create.Edge("TestEdgeClass")
                        .From(vertex1.ORID)
                        .To(vertex2.ORID)
                        .Set("foo", "foo string value")
                        .Set("bar", 12345)
                        .Run();

                    Assert.IsTrue(createdEdge.ORID != null);
                    Assert.AreEqual(createdEdge.GetField<ORID>("in"), vertex2.ORID);
                    Assert.AreEqual(createdEdge.GetField<ORID>("out"), vertex1.ORID);
                    Assert.AreEqual(createdEdge.GetField<string>("foo"), "foo string value");
                    Assert.AreEqual(createdEdge.GetField<int>("bar"), 12345);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateEdgeObjectFromDocumentToDocument()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestProfileClass>()
                        .Extends<OGraphEdge>()
                        .Run();

                    ODocument vertex1 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run();

                    ODocument vertex2 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run();

                    TestProfileClass profile = new TestProfileClass();
                    profile.Name = "Johny";
                    profile.Surname = "Bravo";

                    TestProfileClass createdEdge = database
                        .Create.Edge(profile)
                        .From(vertex1)
                        .To(vertex2)
                        .Run<TestProfileClass>();

                    Assert.IsTrue(createdEdge.ORID != null);
                    Assert.AreEqual(createdEdge.Name, profile.Name);
                    Assert.AreEqual(createdEdge.Surname, profile.Surname);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateEdgeObjectFromObjectToObject()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestProfileClass>()
                        .Extends<OGraphEdge>()
                        .Run();

                    OGraphVertex vertex1 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run<OGraphVertex>();

                    OGraphVertex vertex2 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run<OGraphVertex>();

                    TestProfileClass profile = new TestProfileClass();
                    profile.Name = "Johny";
                    profile.Surname = "Bravo";

                    TestProfileClass createdEdge = database
                        .Create.Edge(profile)
                        .From(vertex1)
                        .To(vertex2)
                        .Run<TestProfileClass>();

                    Assert.IsTrue(createdEdge.ORID != null);
                    Assert.AreEqual(createdEdge.Name, profile.Name);
                    Assert.AreEqual(createdEdge.Surname, profile.Surname);
                }
            }
        }
    }
}
