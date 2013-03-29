using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.SQL
{
    [TestClass]
    public class SqlCreateTests
    {
        [TestMethod]
        public void ShouldCreateClass()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short classId1 = database.Create.Class("TestVertexClass1");
                    Assert.IsTrue(classId1 > 0);

                    short classId2 = database.Create.Class("TestVertexClass2", "OGraphVertex");
                    Assert.AreEqual(classId2, classId1 + 1);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateCluster()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short clusterId = database.Create.Cluster("TestClassCluster", OClusterType.Physical);

                    Assert.IsTrue(clusterId > 0);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateEdge()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    string className = "TestEdgeClass";

                    ODataObject fields = new ODataObject();
                    fields.Set<string>("foo", "foo string value");
                    fields.Set<int>("bar", 12345);

                    // create test class for edges
                    database.Create.Class(className, "OGraphEdge");

                    // create test vertices which will be connected by edge
                    ORecord vertex1 = database.Create.Vertex("OGraphVertex", fields);
                    ORecord vertex2 = database.Create.Vertex("OGraphVertex", fields);

                    // connect previous vertices with edge
                    ORecord edge = database.Create.Edge(className, vertex1.ORID, vertex2.ORID, fields);

                    Assert.AreEqual(edge.HasField("in"), true);
                    Assert.AreEqual(edge.HasField("out"), true);
                    Assert.AreEqual(edge.HasField("foo"), true);
                    Assert.AreEqual(edge.HasField("bar"), true);

                    Assert.AreEqual(edge.GetField<ORID>("out"), vertex1.ORID);
                    Assert.AreEqual(edge.GetField<ORID>("in"), vertex2.ORID);
                    Assert.AreEqual(edge.GetField<string>("foo"), fields.Get<string>("foo"));
                    Assert.AreEqual(edge.GetField<int>("bar"), fields.Get<int>("bar"));
                }
            }
        }

        [TestMethod]
        public void ShouldCreateVertex()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    string className = "TestVertexClass";

                    ODataObject fields = new ODataObject();
                    fields.Set<string>("foo", "foo string value");
                    fields.Set<int>("bar", 12345);

                    // create test class for vertex
                    database.Create.Class(className, "OGraphVertex");

                    // create test vertex from previously created class
                    ORecord vertex = database.Create.Vertex(className, fields);

                    Assert.AreEqual(vertex.HasField("foo"), true);
                    Assert.AreEqual(vertex.HasField("bar"), true);

                    Assert.AreEqual(vertex.GetField<string>("foo"), fields.Get<string>("foo"));
                    Assert.AreEqual(vertex.GetField<int>("bar"), fields.Get<int>("bar"));
                }
            }
        }
    }
}
