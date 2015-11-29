using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlCreateEdgeTests
    {
        [Fact]
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

                    Assert.NotNull(createdEdge.ORID);
                    Assert.Equal("TestEdgeClass", createdEdge.Label);
                    Assert.Equal("TestEdgeClass", createdEdge.OClassName);
                    Assert.Equal(vertex2.ORID, createdEdge.InV);
                    Assert.Equal(vertex1.ORID, createdEdge.OutV);
                }
            }
        }

        [Fact]
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

                    Assert.NotNull(createdEdge.ORID);
                    Assert.Equal("E", createdEdge.Label);
                    Assert.Equal("E", createdEdge.OClassName);
                    Assert.Equal(vertex2.ORID, createdEdge.InV);
                    Assert.Equal(vertex1.ORID, createdEdge.OutV);
                    Assert.Equal(edge.GetField<string>("Foo"), createdEdge.GetField<string>("Foo"));
                    Assert.Equal(edge.GetField<int>("Bar"), createdEdge.GetField<int>("Bar"));
                }
            }
        }

        [Fact]
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

                    Assert.NotNull(createdEdge.ORID);
                    Assert.Equal("TestEdgeClass", createdEdge.Label);
                    Assert.Equal("TestEdgeClass", createdEdge.OClassName);
                    Assert.Equal(vertex2.ORID, createdEdge.InV);
                    Assert.Equal(vertex1.ORID, createdEdge.OutV);
                    Assert.Equal("foo string value", createdEdge.GetField<string>("foo"));
                    Assert.Equal(12345, createdEdge.GetField<int>("bar"));
                }
            }
        }

        [Fact]
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

                    Assert.NotNull(createdEdge.ORID);
                    Assert.Equal(profile.Name, createdEdge.Name);
                    Assert.Equal(profile.Surname, createdEdge.Surname);
                }
            }
        }

        [Fact]
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

                    Assert.NotNull(createdEdge.ORID);
                    Assert.Equal(profile.Name, createdEdge.Name);
                    Assert.Equal(profile.Surname, createdEdge.Surname);
                }
            }
        }
    }
}
