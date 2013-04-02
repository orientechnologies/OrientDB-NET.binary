using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Sql
{
    [TestClass]
    public class SqlCreateTests
    {
        [TestMethod]
        public void ShouldCreateClassUntyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short classId1 = database
                        .Create.Class("TestVertexClass")
                        .Run();
                    Assert.IsTrue(classId1 > 0);

                    short classId2 = database
                        .Create.Class("TestVertexClass2")
                        .Extends("OGraphVertex")
                        .Run();
                    Assert.AreEqual(classId2, classId1 + 1);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateClassTyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short classId1 = database
                        .Create.Class<TestVertexClass>()
                        .Run();
                    Assert.IsTrue(classId1 > 0);

                    short classId2 = database
                        .Create.Class<TestVertexClass2>()
                        .Extends<OGraphVertex>()
                        .Run();
                    Assert.AreEqual(classId2, classId1 + 1);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateClusterUntyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short clusterId1 = database
                        .Create.Cluster("TestVertexClass", OClusterType.Physical)
                        .Run();
                    Assert.IsTrue(clusterId1 > 0);

                    short clusterId2 = database
                        .Create.Cluster("TestClassCluster2", OClusterType.Physical)
                        .Run();
                    Assert.AreEqual(clusterId2, clusterId1 + 1);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateClusterTyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short clusterId1 = database
                        .Create.Cluster<TestVertexClass>(OClusterType.Physical)
                        .Run();
                    Assert.IsTrue(clusterId1 > 0);

                    short clusterId2 = database
                        .Create.Cluster<TestVertexClass2>(OClusterType.Physical)
                        .Run();
                    Assert.AreEqual(clusterId2, clusterId1 + 1);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateEdgeUntyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    ODataObject fields = new ODataObject();
                    fields.Set<string>("foo", "foo string value");
                    fields.Set<int>("bar", 12345);

                    // create test class for edges
                    database
                        .Create.Class("TestEdgeClass")
                        .Extends("OGraphEdge")
                        .Run();

                    // create test vertices which will be connected by edge
                    ORecord vertex1 = database
                        .Create.Vertex("OGraphVertex")
                        .Set(fields)
                        .Run();
                    ORecord vertex2 = database
                        .Create.Vertex("OGraphVertex")
                        .Set(fields)
                        .Run();

                    // connect previous vertices with edge
                    ORecord edge1 = database
                        .Create.Edge("TestEdgeClass")
                        .From(vertex1.ORID)
                        .To(vertex2.ORID)
                        .Set(fields)
                        .Run();

                    Assert.AreEqual(edge1.HasField("in"), true);
                    Assert.AreEqual(edge1.HasField("out"), true);
                    Assert.AreEqual(edge1.HasField("foo"), true);
                    Assert.AreEqual(edge1.HasField("bar"), true);

                    Assert.AreEqual(edge1.GetField<ORID>("out"), vertex1.ORID);
                    Assert.AreEqual(edge1.GetField<ORID>("in"), vertex2.ORID);
                    Assert.AreEqual(edge1.GetField<string>("foo"), fields.Get<string>("foo"));
                    Assert.AreEqual(edge1.GetField<int>("bar"), fields.Get<int>("bar"));

                    // connect previous vertices with another edge using multiple set statements
                    ORecord edge2 = database
                        .Create.Edge("TestEdgeClass")
                        .From(vertex2.ORID)
                        .To(vertex1.ORID)
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    Assert.AreEqual(edge2.HasField("in"), true);
                    Assert.AreEqual(edge2.HasField("out"), true);
                    Assert.AreEqual(edge2.HasField("foo"), true);
                    Assert.AreEqual(edge2.HasField("bar"), true);

                    Assert.AreEqual(edge2.GetField<ORID>("out"), vertex2.ORID);
                    Assert.AreEqual(edge2.GetField<ORID>("in"), vertex1.ORID);
                    Assert.AreEqual(edge2.GetField<string>("foo"), "foo string value2");
                    Assert.AreEqual(edge2.GetField<int>("bar"), 54321);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateEdgeTyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    TestEdgeClass testObj = new TestEdgeClass();
                    testObj.Foo = "foo string value";
                    testObj.Bar = 12345;

                    // create test class for edges
                    database
                        .Create.Class<TestEdgeClass>()
                        .Extends<OGraphEdge>()
                        .Run();

                    // create test vertices which will be connected by edge
                    ORecord vertex1 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run();
                    ORecord vertex2 = database
                        .Create.Vertex<OGraphVertex>()
                        .Run();

                    // connect previous vertices with edge
                    ORecord edge = database
                        .Create.Edge<TestEdgeClass>()
                        .From(vertex1.ORID)
                        .To(vertex2.ORID)
                        .Set(testObj)
                        .Run();

                    Assert.AreEqual(edge.HasField("in"), true);
                    Assert.AreEqual(edge.HasField("out"), true);
                    Assert.AreEqual(edge.HasField("Foo"), true);
                    Assert.AreEqual(edge.HasField("Bar"), true);

                    Assert.AreEqual(edge.GetField<ORID>("out"), vertex1.ORID);
                    Assert.AreEqual(edge.GetField<ORID>("in"), vertex2.ORID);
                    Assert.AreEqual(edge.GetField<string>("Foo"), testObj.Foo);
                    Assert.AreEqual(edge.GetField<int>("Bar"), testObj.Bar);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateVertexUntyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    ODataObject fields = new ODataObject();
                    fields.Set<string>("foo", "foo string value");
                    fields.Set<int>("bar", 12345);

                    List<string> options = new List<string>();
                    options.Add("option1");
                    options.Add("option2");

                    fields.Set<List<string>>("options", options);

                    // create test class for vertex
                    database
                        .Create.Class("TestVertexClass")
                        .Extends("OGraphVertex")
                        .Run();

                    // create test vertex from previously created class
                    ORecord loadedVertex1 = database
                        .Create.Vertex("TestVertexClass")
                        .Set(fields)
                        .Run();

                    Assert.AreEqual(loadedVertex1.HasField("foo"), true);
                    Assert.AreEqual(loadedVertex1.HasField("bar"), true);
                    Assert.AreEqual(loadedVertex1.HasField("options"), true);

                    Assert.AreEqual(loadedVertex1.GetField<string>("foo"), fields.Get<string>("foo"));
                    Assert.AreEqual(loadedVertex1.GetField<int>("bar"), fields.Get<int>("bar"));

                    List<string> loadedOptions = loadedVertex1.GetField<List<string>>("options");
                    Assert.AreEqual(loadedOptions.Count, options.Count);
                    Assert.AreEqual(loadedOptions[0], options[0]);
                    Assert.AreEqual(loadedOptions[1], options[1]);

                    // create test vertex using multiple set statements
                    ORecord loadedVertex2 = database
                        .Create.Vertex("TestVertexClass")
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    Assert.AreEqual(loadedVertex2.HasField("foo"), true);
                    Assert.AreEqual(loadedVertex2.HasField("bar"), true);

                    Assert.AreEqual(loadedVertex2.GetField<string>("foo"), "foo string value2");
                    Assert.AreEqual(loadedVertex2.GetField<int>("bar"), 54321);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateVertexTyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    TestVertexClass testVertex = new TestVertexClass();
                    testVertex.Foo = "foo string value";
                    testVertex.Bar = 12345;

                    // create test class for vertex
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OGraphVertex>()
                        .Run();

                    // create test vertex from previously created class
                    ORecord loadedVertex1 = database
                        .Create.Vertex<TestVertexClass>()
                        .Set(testVertex)
                        .Run();

                    Assert.AreEqual(loadedVertex1.HasField("Foo"), true);
                    Assert.AreEqual(loadedVertex1.HasField("Bar"), true);

                    Assert.AreEqual(loadedVertex1.GetField<string>("Foo"), testVertex.Foo);
                    Assert.AreEqual(loadedVertex1.GetField<int>("Bar"), testVertex.Bar);
                }
            }
        }
    }
}
