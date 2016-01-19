using System.Linq;
using NUnit.Framework;
using Orient.Client;
using System.Collections.Generic;

namespace Orient.Tests.Query
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
                    orid = testVertex.ORID;
                }

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {

                    OVertex v = database.Load.ORID(orid).Run().To<OVertex>();
                    v.SetField("foobar", "blah");
                    database.Transaction.Update(v);

                    database.Transaction.Commit();
                }

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {

                    OVertex v = database.Load.ORID(orid).Run().To<OVertex>();
                    Assert.AreEqual("blah", v.GetField<string>("foobar"));
                }

            }
        }

        [Test]
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
                    Assert.AreEqual("TestVertexClass", createdVertex.OClassName);
                    Assert.AreEqual("foo string value", createdVertex.GetField<string>("foo"));
                    Assert.AreEqual(12345, createdVertex.GetField<int>("bar"));
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

        [Test]
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
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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

                    Assert.AreEqual(testVertex2.ORID, testVertex1.OutE.First());
                    Assert.AreEqual(testVertex1.ORID, testVertex2.InE.First());

                    var createdVertices = database.Select().From("V").ToList<OVertex>();
                    Assert.AreEqual(2, createdVertices.Count);

                    Assert.AreEqual(createdVertices[1].ORID, createdVertices[0].OutE.First());
                    Assert.AreEqual(createdVertices[0].ORID, createdVertices[1].InE.First());

                }
            }
        }

        [Test]
        public void TestCreateVerticesAndHeavyweightEdge()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OVertex>()
                        .Run();
                    database
                        .Create.Class<TestEdgeClass>()
                        .Extends<OEdge>()
                        .Run();

                    var testVertex1 = CreateTestVertex(1);
                    var testVertex2 = CreateTestVertex(2);
                    var testEdge = new TestEdgeClass();
                    testEdge.SetField("item", 1);

                    database.Transaction.Add(testVertex1);
                    database.Transaction.Add(testVertex2);
                    database.Transaction.AddEdge(testEdge, testVertex1, testVertex2);

                    Assert.AreEqual(testVertex1.ORID, testEdge.OutV);
                    Assert.AreEqual(testVertex2.ORID, testEdge.InV);

                    database.Transaction.Commit();

                    var createdVertex1 = database.Select().From("V").Where("bar").Equals(1).ToList<OVertex>().First();
                    var createdVertex2 = database.Select().From("V").Where("bar").Equals(2).ToList<OVertex>().First();

                    var createdEdge = database.Select().From("E").Where("item").Equals(1).ToList<OEdge>().First();
                    Assert.That(createdEdge.OutV, Is.EqualTo(createdVertex1.ORID));
                    Assert.That(createdEdge.InV, Is.EqualTo(createdVertex2.ORID));

                    var testEdge2 = new TestEdgeClass();
                    testEdge2.SetField("item", 2);
                    database.Transaction.AddEdge(testEdge2, createdVertex2, createdVertex1);
                    database.Transaction.Commit();

                    createdVertex1 = database.Select().From("V").Where("bar").Equals(1).ToList<OVertex>().First();
                    createdVertex2 = database.Select().From("V").Where("bar").Equals(2).ToList<OVertex>().First();
                    var createdEdge2 = database.Select().From("E").Where("item").Equals(2).ToList<OEdge>().First();

                    Assert.That(createdEdge2.OutV, Is.EqualTo(createdVertex2.ORID));
                    Assert.That(createdEdge2.InV, Is.EqualTo(createdVertex1.ORID));
                }
            }
        }

        [Test]
        public void DeleteEdgeTest()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                // Arrange
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OVertex>()
                        .Run();
                    database
                        .Create.Class<TestEdgeClass>()
                        .Extends<OEdge>()
                        .Run();

                    var vOut = CreateTestVertex(1);
                    var vIn = CreateTestVertex(2);
                    var edge = new TestEdgeClass();
                    edge.SetField("item", 1);

                    database.Transaction.Add(vOut);
                    database.Transaction.Add(vIn);
                    database.Transaction.AddEdge(edge, vOut, vIn);

                    database.Transaction.Commit();

                    // Validate arrange
                    var createdVertex1 = database.Select().From("V").Where("bar").Equals(1).ToList<OVertex>().First();
                    var createdVertex2 = database.Select().From("V").Where("bar").Equals(2).ToList<OVertex>().First();
                    var createdEdge = database.Select().From("E").Where("item").Equals(1).ToList<OEdge>().First();
                    Assert.That(createdEdge.OutV, Is.EqualTo(createdVertex1.ORID));
                    Assert.That(createdEdge.InV, Is.EqualTo(createdVertex2.ORID));
                }

                // Act
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    var createdEdge = database.Select().From("E").Where("item").Equals(1).ToList<OEdge>().First();

                    database.Transaction.DeleteEdge(createdEdge);
                    database.Transaction.Commit();
                }

                // Assert
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {

                    var vOut = database.Select().From("V").Where("bar").Equals(1).ToList<OVertex>().First();
                    var vIn = database.Select().From("V").Where("bar").Equals(2).ToList<OVertex>().First();

                    var deletedEdge = database.Select().From("E").Where("item").Equals(1).ToList<OEdge>().FirstOrDefault();

                    Assert.IsNull(deletedEdge);
                    Assert.That(vOut.GetField<HashSet<object>>("out_TestEdgeClass").Count, Is.EqualTo(0));
                    Assert.That(vIn.GetField<HashSet<object>>("in_TestEdgeClass").Count, Is.EqualTo(0));
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
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database.Create.Class<Widget>().Extends<OVertex>().Run();


                    var w1 = new Widget() { Foo = "foo", Bar = 1 };
                    var w2 = new Widget() { Foo = "woo", Bar = 2 };

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
