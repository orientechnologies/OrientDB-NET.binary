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
                        .Extends<OEdge>()
                        .Run();

                    database
                        .Create.Cluster("TestCluster", OClusterType.Physical)
                        .Run();

                    OVertex vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Run();

                    OVertex vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Run();

                    OEdge createdEdge = database
                        .Create.Edge("TestEdgeClass")
                        .Cluster("TestCluster")
                        .From(vertex1.ORID)
                        .To(vertex2.ORID)
                        .Run();

                    Assert.IsTrue(!string.IsNullOrEmpty(createdEdge.ORID.ToString()));
                    Assert.AreEqual(createdEdge.Label, "TestEdgeClass");
                    Assert.AreEqual(createdEdge.OClassName, "TestEdgeClass");
                    Assert.AreEqual(createdEdge.InV, vertex2.ORID);
                    Assert.AreEqual(createdEdge.OutV, vertex1.ORID);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateEdgeFromOEdge()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    OVertex vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Run();

                    OVertex vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Run();

                    OEdge edge = new OEdge();
                    edge.SetField("Foo", "foo string value");
                    edge.SetField("Bar", 12345);

                    OEdge createdEdge = database
                        .Create.Edge(edge)
                        .From(vertex1)
                        .To(vertex2)
                        .Run();

                    Assert.IsTrue(!string.IsNullOrEmpty(createdEdge.ORID.ToString()));
                    Assert.AreEqual(createdEdge.Label, "E");
                    Assert.AreEqual(createdEdge.OClassName, "E");
                    Assert.AreEqual(createdEdge.InV, vertex2.ORID);
                    Assert.AreEqual(createdEdge.OutV, vertex1.ORID);
                    Assert.AreEqual(createdEdge.GetField<string>("Foo"), edge.GetField<string>("Foo"));
                    Assert.AreEqual(createdEdge.GetField<int>("Bar"), edge.GetField<int>("Bar"));
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
                        .Extends<OEdge>()
                        .Run();

                    OVertex vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Run();

                    OVertex vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Run();

                    OEdge createdEdge = database
                        .Create.Edge("TestEdgeClass")
                        .From(vertex1.ORID)
                        .To(vertex2.ORID)
                        .Set("foo", "foo string value")
                        .Set("bar", 12345)
                        .Run();

                    Assert.IsTrue(!string.IsNullOrEmpty(createdEdge.ORID.ToString()));
                    Assert.AreEqual(createdEdge.Label, "TestEdgeClass");
                    Assert.AreEqual(createdEdge.OClassName, "TestEdgeClass");
                    Assert.AreEqual(createdEdge.InV, vertex2.ORID);
                    Assert.AreEqual(createdEdge.OutV, vertex1.ORID);
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
                        .Extends<OEdge>()
                        .Run();

                    OVertex vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Run();

                    OVertex vertex2 = database
                        .Create.Vertex<OVertex>()
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
                        .Extends<OEdge>()
                        .Run();

                    OVertex vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Run<OVertex>();

                    OVertex vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Run<OVertex>();

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
