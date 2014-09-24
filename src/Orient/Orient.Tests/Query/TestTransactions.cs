using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class TestTransactions
    {
        [TestMethod]
        public void TestCreateVertex()
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

                    OVertex testVertex = new OVertex();
                    testVertex.OClassName = "TestVertexClass";
                    testVertex.SetField("foo", "foo string value");
                    testVertex.SetField("bar", 12345);

                    Assert.AreEqual(null, testVertex.ORID);

                    database.Transaction.Add(testVertex);

                    Assert.IsNotNull(testVertex.ORID);
                    Assert.IsTrue(testVertex.ORID.ClusterPosition < 0);
                    Assert.AreEqual(-2, testVertex.ORID.ClusterPosition);

                    database.Transaction.Commit();

                    Assert.IsNotNull(testVertex.ORID);
                    Assert.AreEqual(database.GetClusterIdFor("TestVertexClass"), testVertex.ORID.ClusterId);

                    var createdVertex = database.Load.ORID(testVertex.ORID).Run().To<OVertex>();

                    Assert.IsTrue(createdVertex.ORID != null);
                    Assert.AreEqual(createdVertex.OClassName, "TestVertexClass");
                    Assert.AreEqual(createdVertex.GetField<string>("foo"), "foo string value");
                    Assert.AreEqual(createdVertex.GetField<int>("bar"), 12345);
                }

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                 

                    OVertex testVertex = new OVertex();
                    testVertex.OClassName = "TestVertexClass";
                    testVertex.SetField("foo", "foo string value");
                    testVertex.SetField("bar", 12345);

                    Assert.AreEqual(null, testVertex.ORID);

                    database.Transaction.Add(testVertex);

                    Assert.IsNotNull(testVertex.ORID);
                    Assert.IsTrue(testVertex.ORID.ClusterPosition < 0);
                    Assert.AreEqual(-2, testVertex.ORID.ClusterPosition);

                    database.Transaction.Commit();

                    Assert.IsNotNull(testVertex.ORID);
                    Assert.AreEqual(database.GetClusterIdFor("TestVertexClass"), testVertex.ORID.ClusterId);
                    Assert.AreNotEqual(-2, testVertex.ORID.ClusterPosition);

                    var createdVertex = database.Load.ORID(testVertex.ORID).Run().To<OVertex>();

                    Assert.IsTrue(createdVertex.ORID != null);
                    Assert.AreEqual(createdVertex.OClassName, "TestVertexClass");
                    Assert.AreEqual(createdVertex.GetField<string>("foo"), "foo string value");
                    Assert.AreEqual(createdVertex.GetField<int>("bar"), 12345);
                }

            }
        }

        [TestMethod]
        public void TestCreateManyVertices()
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

                    for (int i = 0; i < 1000; i++)
                    {
                        OVertex testVertex = new OVertex();
                        testVertex.OClassName = "TestVertexClass";
                        testVertex.SetField("foo", "foo string value");
                        testVertex.SetField("bar", i);
                        database.Transaction.Add(testVertex);
                    }

                    database.Transaction.Commit();


                    var createdVertices = database.Select().From("V").ToList();
                    Assert.AreEqual(1000, createdVertices.Count);

                }
            }
        }

    }
}
