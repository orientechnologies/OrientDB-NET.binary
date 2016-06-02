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
                    var transaction = database.CreateTransaction();

                    OVertex testVertex = new OVertex();
                    testVertex.OClassName = "TestVertexClass";
                    testVertex.SetField("foo", "foo string value");
                    testVertex.SetField("bar", 12345);

                    Assert.AreEqual(null, testVertex.ORID);

                    transaction.Add(testVertex);

                    Assert.IsNotNull(testVertex.ORID);
                    Assert.IsTrue(testVertex.ORID.ClusterPosition < 0);
                    Assert.AreEqual(-2, testVertex.ORID.ClusterPosition);

                    transaction.Commit();
                    orid = testVertex.ORID;
                }

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {

                    OVertex v = database.Load.ORID(orid).Run().To<OVertex>();
                    v.SetField("foobar", "blah");
                    var transaction = database.CreateTransaction();

                    transaction.Update(v);

                    transaction.Commit();
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
                    var transaction = database.CreateTransaction();

                    OVertex testVertex = new OVertex();
                    testVertex.OClassName = "TestVertexClass";
                    testVertex.SetField("foo", "foo string value");
                    testVertex.SetField("bar", 12345);

                    Assert.AreEqual(null, testVertex.ORID);

                    transaction.Add(testVertex);

                    Assert.IsNotNull(testVertex.ORID);
                    Assert.IsTrue(testVertex.ORID.ClusterPosition < 0);
                    Assert.AreEqual(-2, testVertex.ORID.ClusterPosition);

                    transaction.Commit();

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

                    var transaction = database.CreateTransaction();

                    transaction.Add(testVertex);

                    Assert.IsNotNull(testVertex.ORID);
                    Assert.IsTrue(testVertex.ORID.ClusterPosition < 0);
                    Assert.AreEqual(-2, testVertex.ORID.ClusterPosition);

                    transaction.Commit();

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
                    var transaction = database.CreateTransaction();

                    for (int i = 0; i < 1000; i++)
                    {
                        OVertex testVertex = new OVertex();
                        testVertex.OClassName = "TestVertexClass";
                        testVertex.SetField("foo", "foo string value");
                        testVertex.SetField("bar", i);
                        transaction.Add(testVertex);
                    }

                    transaction.Commit();


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
                    var transaction = database.CreateTransaction();


                    var testVertex1 = CreateTestVertex(1);
                    var testVertex2 = CreateTestVertex(2);
                    transaction.Add(testVertex1);
                    transaction.Add(testVertex2);
                    testVertex1.OutE.Add(testVertex2.ORID);
                    testVertex2.InE.Add(testVertex1.ORID);


                    transaction.Commit();

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
                    var transaction = database.CreateTransaction();

                    var testVertex1 = CreateTestVertex(1);
                    var testVertex2 = CreateTestVertex(2);
                    var testEdge = new TestEdgeClass();
                    testEdge.SetField("item", 1);

                    transaction.Add(testVertex1);
                    transaction.Add(testVertex2);
                    transaction.AddEdge(testEdge, testVertex1, testVertex2);

                    Assert.AreEqual(testVertex1.ORID, testEdge.OutV);
                    Assert.AreEqual(testVertex2.ORID, testEdge.InV);

                    transaction.Commit();

                    var createdVertex1 = database.Select().From("V").Where("bar").Equals(1).ToList<OVertex>().First();
                    var createdVertex2 = database.Select().From("V").Where("bar").Equals(2).ToList<OVertex>().First();

                    var createdEdge = database.Select().From("E").Where("item").Equals(1).ToList<OEdge>().First();
                    Assert.That(createdEdge.OutV, Is.EqualTo(createdVertex1.ORID));
                    Assert.That(createdEdge.InV, Is.EqualTo(createdVertex2.ORID));

                    var testEdge2 = new TestEdgeClass();
                    testEdge2.SetField("item", 2);
                    transaction.AddEdge(testEdge2, createdVertex2, createdVertex1);
                    transaction.Commit();

                    createdVertex1 = database.Select().From("V").Where("bar").Equals(1).ToList<OVertex>().First();
                    createdVertex2 = database.Select().From("V").Where("bar").Equals(2).ToList<OVertex>().First();
                    var createdEdge2 = database.Select().From("E").Where("item").Equals(2).ToList<OEdge>().First();

                    Assert.That(createdEdge2.OutV, Is.EqualTo(createdVertex2.ORID));
                    Assert.That(createdEdge2.InV, Is.EqualTo(createdVertex1.ORID));
                }
            }
        }

        /// <summary>
        /// Tests the DeleteEdge method of the OTransaction object.
        /// </summary>
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
                    var transaction = database.CreateTransaction();

                    var vOut = CreateTestVertex(1);
                    var vIn = CreateTestVertex(2);
                    var edge = new TestEdgeClass();
                    edge.SetField("item", 1);

                    transaction.Add(vOut);
                    transaction.Add(vIn);
                    transaction.AddEdge(edge, vOut, vIn);

                    transaction.Commit();

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

                    var transaction = database.CreateTransaction();
                    transaction.DeleteEdge(createdEdge);
                    transaction.Commit();
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

        /// <summary>
        /// Tests that calling delete on an edge updates the in_ and out_ properties of the end verticles (inV and outV) references too.
        /// </summary>
        [Test]
        public void TestThatCallingDeleteOnAnEdgeUpdatesTheInAndOutReferencesToo()
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
                    var transaction = database.CreateTransaction();

                    var vOut = CreateTestVertex(1);
                    var vIn = CreateTestVertex(2);
                    var edge = new TestEdgeClass();
                    edge.SetField("item", 1);

                    transaction.Add(vOut);
                    transaction.Add(vIn);
                    transaction.AddEdge(edge, vOut, vIn);

                    transaction.Commit();

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

                    var transaction = database.CreateTransaction();
                    transaction.Delete(createdEdge);
                    transaction.Commit();
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

        /// <summary>
        /// Tests that the edge rids are added to the Verticles edge lists (in_... and out_ ... fields)
        /// when the edge list is not empty on the verticle.
        /// </summary>
        [Test]
        public void TestAddEdgeToVerticlaWhichAlreadyHasEdges()
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
                    var transaction = database.CreateTransaction();

                    var fromV = CreateTestVertex(1);
                    var toV = CreateTestVertex(2);
                    var firstEdge = new TestEdgeClass();
                    firstEdge.SetField("item", 1);

                    transaction.Add(fromV);
                    transaction.Add(toV);
                    transaction.AddEdge(firstEdge, fromV, toV);

                    Assert.AreEqual(fromV.ORID, firstEdge.OutV);
                    Assert.AreEqual(toV.ORID, firstEdge.InV);

                    transaction.Commit();
                }
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    var fromV = database.Select().From("V").Where("bar").Equals(1).ToList<OVertex>().First();
                    var toV = database.Select().From("V").Where("bar").Equals(2).ToList<OVertex>().First();

                    var secondEdge = new TestEdgeClass();
                    secondEdge.SetField("item", 2);
                    var transaction = database.CreateTransaction();
                    transaction.AddEdge(secondEdge, fromV, toV);
                    transaction.Commit();

                }
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    var fromV = database.Select().From("V").Where("bar").Equals(1).ToList<OVertex>().First();
                    var toV = database.Select().From("V").Where("bar").Equals(2).ToList<OVertex>().First();
                    var secondEdge = database.Select().From("E").Where("item").Equals(2).ToList<OEdge>().First();

                    Assert.That(secondEdge.OutV, Is.EqualTo(fromV.ORID));
                    Assert.That(secondEdge.InV, Is.EqualTo(toV.ORID));
                    Assert.That(fromV.GetField<HashSet<object>>("out_TestEdgeClass").Count, Is.EqualTo(2));
                    Assert.That(toV.GetField<HashSet<object>>("in_TestEdgeClass").Count, Is.EqualTo(2));
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
                    var transaction = database.CreateTransaction();


                    var w1 = new Widget() { Foo = "foo", Bar = 1 };
                    var w2 = new Widget() { Foo = "woo", Bar = 2 };

                    transaction.Add(w1);
                    transaction.Add(w2);
                    w1.OtherWidget = w2.ORID;

                    transaction.Commit();

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
