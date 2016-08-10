using NUnit.Framework;
using Orient.Client;
using System.Linq;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class TestTransactions
    {
        [Test]
        public void TestUpdateVertex()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                ORID orid;
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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
                    orid = testVertex.ORID;
                }

                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {

                    OVertex v = database.Load.ORID(orid).Run().To<OVertex>();
                    v.SetField("foobar", "blah");
                    database.Transaction.Update(v);

                    database.Transaction.Commit();
                }

                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {

                    OVertex v = database.Load.ORID(orid).Run().To<OVertex>();
                    Assert.AreEqual("blah", v.GetField<string>("foobar"));
                }

            }
        }

        [Test]
        [Ignore("Need to discuss with Jonathon the fix for collections.")]
        public void TestCreateVertex()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

        [Test]
        public void TestCreateManyVertices()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

                    for (int i = 0; i < 1000; i++)
                    {
                        Assert.AreEqual(i, createdVertices[i].GetField<int>("bar"));
                    }

                }
            }
        }


        [Test]
        public void TestCreateVerticesAndEdge()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestVertexClass")
                        .Extends<OVertex>()
                        .Run();

                   
                    var testVertex1 = CreateTestVertex(1);
                    var testVertex2 = CreateTestVertex(2);
                    database.Transaction.Add(testVertex1);
                    database.Transaction.Add(testVertex2);
                    testVertex1.OutE.Add(testVertex2.ORID);
                    testVertex2.InE.Add(testVertex1.ORID);
                   

                    database.Transaction.Commit();

                    Assert.AreEqual( testVertex2.ORID, testVertex1.OutE.First());
                    Assert.AreEqual(testVertex1.ORID, testVertex2.InE.First());

                    var createdVertices = database.Select().From("V").ToList<OVertex>();
                    Assert.AreEqual(2, createdVertices.Count);

                    Assert.AreEqual(createdVertices[1].ORID, createdVertices[0].OutE.First());
                    Assert.AreEqual(createdVertices[0].ORID, createdVertices[1].InE.First());

                }
            }
        }

        class Widget : OBaseRecord
        {
            public string Foo { get; set; }
            public int Bar { get; set; }
            public ORID OtherWidget { get; set; }
        }

        [Test]
        public void TestTypedCreateVerticesAndLinks()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database.Create.Class<Widget>().Extends<OVertex>().Run();


                    var w1 = new Widget() {Foo = "foo", Bar = 1};
                    var w2 = new Widget() {Foo = "woo", Bar = 2};

                    database.Transaction.Add(w1);
                    database.Transaction.Add(w2);
                    w1.OtherWidget = w2.ORID;

                    database.Transaction.Commit();

                    Assert.AreEqual(w2.ORID, w1.OtherWidget);

                    var createdVertices = database.Select().From<Widget>().ToList<Widget>();
                    Assert.AreEqual(2, createdVertices.Count);

                    var withLink = createdVertices.First(x => x.OtherWidget != null);
                    var noLink = createdVertices.First(x => x.OtherWidget == null);


                    Assert.AreEqual(noLink.ORID, withLink.OtherWidget);

                }
            }
        }

        private static OVertex CreateTestVertex(int iBar)
        {
            OVertex testVertex = new OVertex();
            testVertex.OClassName = "TestVertexClass";
            testVertex.SetField("foo", "foo string value");
            testVertex.SetField("bar", iBar);
            return testVertex;
        }
    }
}
