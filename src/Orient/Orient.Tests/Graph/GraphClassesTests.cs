using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Graph
{
    [TestClass]
    public class GraphClassesTests
    {
        [TestMethod]
        public void ShouldNotThrowExceptionWhenCreatingVerticesWithNonASCIIChars()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    //This commadn contains a character that will take up more than one byte
                    OVertex vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Set("Name", "René")
                        .Run<OVertex>();
                    //This command will throw exception if bytearray lengths and string lenghts are mixed up in protocol
                    OVertex vertex2 = database.Create.Vertex<OVertex>().Set("Name", "test").Run<OVertex>();
                }
            }

        }
        [TestMethod]
        public void ShouldCreateVerticesWithEdge()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    OVertex vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Set("Foo", "foo string value1")
                        .Set("Bar", 12345)
                        .Run<OVertex>();

                    Assert.IsTrue(!string.IsNullOrEmpty(vertex1.ORID.ToString()));
                    Assert.AreEqual("V", vertex1.OClassName);
                    Assert.AreEqual("foo string value1", vertex1.GetField<string>("Foo"));
                    Assert.AreEqual(12345, vertex1.GetField<int>("Bar"));

                    OVertex vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Set("Foo", "foo string value2")
                        .Set("Bar", 54321)
                        .Run<OVertex>();

                    Assert.IsTrue(!string.IsNullOrEmpty(vertex2.ORID.ToString()));
                    Assert.AreEqual("V", vertex2.OClassName);
                    Assert.AreEqual("foo string value2", vertex2.GetField<string>("Foo"));
                    Assert.AreEqual(54321, vertex2.GetField<int>("Bar"));

                    OVertex vertex3 = database
                        .Create.Vertex<OVertex>()
                        .Set("Foo", "foo string value3")
                        .Set("Bar", 347899)
                        .Run<OVertex>();

                    Assert.IsTrue(!string.IsNullOrEmpty(vertex3.ORID.ToString()));
                    Assert.AreEqual("V", vertex3.OClassName);
                    Assert.AreEqual("foo string value3", vertex3.GetField<string>("Foo"));
                    Assert.AreEqual(347899, vertex3.GetField<int>("Bar"));

                    OEdge edge1 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("Foo", "foo string value3")
                        .Set("Bar", 123)
                        .Run<OEdge>();

                    Assert.IsTrue(!string.IsNullOrEmpty(edge1.ORID.ToString()));
                    Assert.AreEqual("E", edge1.Label);
                    Assert.AreEqual("E", edge1.OClassName);
                    Assert.AreEqual(vertex2.ORID, edge1.InV);
                    Assert.AreEqual(vertex1.ORID, edge1.OutV);
                    Assert.AreEqual("foo string value3", edge1.GetField<string>("Foo"));
                    Assert.AreEqual(123, edge1.GetField<int>("Bar"));

                    OEdge edge2 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex3)
                        .Set("Foo", "foo string value4")
                        .Set("Bar", 245)
                        .Run<OEdge>();

                    Assert.IsTrue(!string.IsNullOrEmpty(edge2.ORID.ToString()));
                    Assert.AreEqual("E", edge2.Label);
                    Assert.AreEqual("E", edge2.OClassName);
                    Assert.AreEqual(vertex3.ORID, edge2.InV);
                    Assert.AreEqual(vertex1.ORID, edge2.OutV);
                    Assert.AreEqual("foo string value4", edge2.GetField<string>("Foo"));
                    Assert.AreEqual(245, edge2.GetField<int>("Bar"));

                    OVertex loadedVertex1 = database
                        .Select()
                        .From(vertex1)
                        .ToList<OVertex>().First();

                    Assert.IsTrue(!string.IsNullOrEmpty(loadedVertex1.ORID.ToString()));
                    Assert.AreEqual("V", loadedVertex1.OClassName);
                    Assert.AreEqual(0, loadedVertex1.InE.Count);
                    Assert.AreEqual(2, loadedVertex1.OutE.Count);
                    Assert.IsTrue(loadedVertex1.OutE.Contains(edge1.ORID));
                    Assert.IsTrue(loadedVertex1.OutE.Contains(edge2.ORID));
                    Assert.AreEqual(vertex1.GetField<string>("Foo"), loadedVertex1.GetField<string>("Foo"));
                    Assert.AreEqual(vertex1.GetField<int>("Bar"), loadedVertex1.GetField<int>("Bar"));

                    OVertex loadedVertex2 = database
                        .Select()
                        .From(vertex2)
                        .ToList<OVertex>().First();

                    Assert.IsTrue(!string.IsNullOrEmpty(loadedVertex2.ORID.ToString()));
                    Assert.AreEqual("V", loadedVertex2.OClassName);
                    Assert.AreEqual(0, loadedVertex2.OutE.Count);
                    Assert.AreEqual(1, loadedVertex2.InE.Count);
                    Assert.IsTrue(loadedVertex2.InE.Contains(edge1.ORID));
                    Assert.AreEqual(vertex2.GetField<string>("Foo"), loadedVertex2.GetField<string>("Foo"));
                    Assert.AreEqual(vertex2.GetField<int>("Bar"), loadedVertex2.GetField<int>("Bar"));
                }
            }
        }
    }
}
