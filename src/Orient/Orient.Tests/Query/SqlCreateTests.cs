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
                string className = "TestVertexClass";

                string sql1 = OSQL.Create.Class(className);
                Assert.AreEqual(sql1, "CREATE CLASS " + className);

                string sql2 = OSQL.Create.Class(className, "OGraphVertex");
                Assert.AreEqual(sql2, "CREATE CLASS " + className + " EXTENDS OGraphVertex");

                string sql3 = OSQL.Create.Class(className, "OGraphVertex", 10);
                Assert.AreEqual(sql3, "CREATE CLASS " + className + " EXTENDS OGraphVertex CLUSTER 10");

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    OCommandResult result = database.Command(sql1);

                    int classId = int.Parse(result.ToDataObject().Get<string>("Content"));

                    Assert.IsTrue(classId > 0);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateCluster()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                string sql = OSQL.Create.Cluster("TestClassCluster", OClusterType.Physical);
                Assert.AreEqual(sql, "CREATE CLUSTER TestClassCluster PHYSICAL");

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    OCommandResult result = database.Command(sql);

                    int clusterId = int.Parse(result.ToDataObject().Get<string>("Content"));

                    Assert.IsTrue(clusterId > 0);
                }
            }
        }

        [TestMethod]
        public void ShouldCreateEdge()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                string className = "TestEdgeClass";

                ODataObject fields = new ODataObject();
                fields.Set<string>("foo", "foo string value");
                fields.Set<int>("bar", 12345);

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database.Command(OSQL.Create.Class(className, "OGraphEdge"));

                    ORecord vertex1 = database.Command(
                        OSQL.Create.Vertex("OGraphVertex", fields)
                    ).ToSingle();

                    ORecord vertex2 = database.Command(
                        OSQL.Create.Vertex("OGraphVertex", fields)
                    ).ToSingle();

                    string sql1 = OSQL.Create.Edge(className, vertex1.ORID, vertex2.ORID);
                    Assert.AreEqual(sql1, "CREATE EDGE " + className + " FROM " + vertex1.ORID.ToString() + " TO " + vertex2.ORID.ToString());

                    string sql2 = OSQL.Create.Edge(className, vertex1.ORID, vertex2.ORID, fields);
                    Assert.AreEqual(sql2, "CREATE EDGE " + className + " FROM " + vertex1.ORID.ToString() + " TO " + vertex2.ORID.ToString() + " SET foo = 'foo string value', bar = 12345");

                    string sql3 = OSQL.Create.Edge(className, "TestClassCluster", vertex1.ORID, vertex2.ORID);
                    Assert.AreEqual(sql3, "CREATE EDGE " + className + " CLUSTER TestClassCluster FROM " + vertex1.ORID.ToString() + " TO " + vertex2.ORID.ToString());

                    string sql4 = OSQL.Create.Edge(className, "TestClassCluster", vertex1.ORID, vertex2.ORID, fields);
                    Assert.AreEqual(sql4, "CREATE EDGE " + className + " CLUSTER TestClassCluster FROM " + vertex1.ORID.ToString() + " TO " + vertex2.ORID.ToString() + " SET foo = 'foo string value', bar = 12345");

                    ORecord edge = database.Command(sql2).ToSingle();

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
                string className = "TestVertexClass";

                ODataObject fields = new ODataObject();
                fields.Set<string>("foo", "foo string value");
                fields.Set<int>("bar", 12345);

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database.Command(OSQL.Create.Class(className, "OGraphVertex"));

                    string sql1 = OSQL.Create.Vertex(className, "TestClassCluster");
                    Assert.AreEqual(sql1, "CREATE VERTEX " + className + " CLUSTER TestClassCluster");

                    string sql2 = OSQL.Create.Vertex(className, fields);
                    Assert.AreEqual(sql2, "CREATE VERTEX " + className + " SET foo = 'foo string value', bar = 12345");

                    string sql3 = OSQL.Create.Vertex(className, "TestClassCluster", fields);
                    Assert.AreEqual(sql3, "CREATE VERTEX " + className + " CLUSTER TestClassCluster SET foo = 'foo string value', bar = 12345");

                    ORecord vertex = database.Command(sql2).ToSingle();

                    Assert.AreEqual(vertex.HasField("foo"), true);
                    Assert.AreEqual(vertex.HasField("bar"), true);

                    Assert.AreEqual(vertex.GetField<string>("foo"), fields.Get<string>("foo"));
                    Assert.AreEqual(vertex.GetField<int>("bar"), fields.Get<int>("bar"));
                }
            }
        }
    }
}
