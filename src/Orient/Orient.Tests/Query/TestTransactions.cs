using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class TestTransactions
    {
        [TestMethod]
        public void TestCreateVertices()
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
                    Assert.AreEqual(-1, testVertex.ORID.ClusterId);

                    database.Transaction.Commit();

                    Assert.IsNotNull(testVertex.ORID);
                    Assert.AreNotEqual(-1, testVertex.ORID.ClusterId);

                    var createdVertex = database.Load.ORID(testVertex.ORID).Run().To<OVertex>();

                    Assert.IsTrue(createdVertex.ORID != null);
                    Assert.AreEqual(createdVertex.OClassName, "TestVertexClass");
                    Assert.AreEqual(createdVertex.GetField<string>("foo"), "foo string value");
                    Assert.AreEqual(createdVertex.GetField<int>("bar"), 12345);
                }
            }
        }
    }
}
