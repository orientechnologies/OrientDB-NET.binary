using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class SqlCreateEdgeTests
    {
        [Test]
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
                    
                    var res = database.Command("alter class TestEdgeClass addcluster testcluster");

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

                    Assert.IsNotNull(createdEdge.ORID);
                    Assert.AreEqual("TestEdgeClass", createdEdge.Label);
                    Assert.AreEqual("TestEdgeClass", createdEdge.OClassName);
                    Assert.AreEqual(vertex2.ORID, createdEdge.InV);
                    Assert.AreEqual(vertex1.ORID, createdEdge.OutV);
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdEdge.ORID);
                    Assert.AreEqual("E", createdEdge.Label);
                    Assert.AreEqual("E", createdEdge.OClassName);
                    Assert.AreEqual(vertex2.ORID, createdEdge.InV);
                    Assert.AreEqual(vertex1.ORID, createdEdge.OutV);
                    Assert.AreEqual(edge.GetField<string>("Foo"), createdEdge.GetField<string>("Foo"));
                    Assert.AreEqual(edge.GetField<int>("Bar"), createdEdge.GetField<int>("Bar"));
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdEdge.ORID);
                    Assert.AreEqual("TestEdgeClass", createdEdge.Label);
                    Assert.AreEqual("TestEdgeClass", createdEdge.OClassName);
                    Assert.AreEqual(vertex2.ORID, createdEdge.InV);
                    Assert.AreEqual(vertex1.ORID, createdEdge.OutV);
                    Assert.AreEqual("foo string value", createdEdge.GetField<string>("foo"));
                    Assert.AreEqual(12345, createdEdge.GetField<int>("bar"));
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdEdge.ORID);
                    Assert.AreEqual(profile.Name, createdEdge.Name);
                    Assert.AreEqual(profile.Surname, createdEdge.Surname);
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdEdge.ORID);
                    Assert.AreEqual(profile.Name, createdEdge.Name);
                    Assert.AreEqual(profile.Surname, createdEdge.Surname);
                }
            }
        }
    }
}
