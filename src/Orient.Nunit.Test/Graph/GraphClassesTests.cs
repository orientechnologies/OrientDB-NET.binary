using NUnit.Framework;
using Orient.Client;
using System.Linq;

namespace Orient.Nunit.Test.Graph
{
   [TestFixture]
    public class GraphClassesTests
    {
        [Test]
        public void ShouldNotThrowExceptionWhenCreatingVerticesWithNonASCIIChars()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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
        [Test]
        public void ShouldCreateVerticesWithEdge()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    OVertex vertex1 = database
                        .Create.Vertex<OVertex>()
                        .Set("Foo", "foo string value1")
                        .Set("Bar", 12345)
                        .Run<OVertex>();

                    Assert.IsTrue(!string.IsNullOrEmpty(vertex1.ORID.ToString()));
                    Assert.AreEqual(vertex1.OClassName, "V");
                    Assert.AreEqual(vertex1.GetField<string>("Foo"), "foo string value1");
                    Assert.AreEqual(vertex1.GetField<int>("Bar"), 12345);

                    OVertex vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Set("Foo", "foo string value2")
                        .Set("Bar", 54321)
                        .Run<OVertex>();

                    Assert.IsTrue(!string.IsNullOrEmpty(vertex2.ORID.ToString()));
                    Assert.AreEqual(vertex2.OClassName, "V");
                    Assert.AreEqual(vertex2.GetField<string>("Foo"), "foo string value2");
                    Assert.AreEqual(vertex2.GetField<int>("Bar"), 54321);
                    
                    OVertex vertex3 = database
                        .Create.Vertex<OVertex>()
                        .Set("Foo", "foo string value3")
                        .Set("Bar", 347899)
                        .Run<OVertex>();
                    
                    Assert.IsTrue(!string.IsNullOrEmpty(vertex3.ORID.ToString()));
                    Assert.AreEqual(vertex3.OClassName, "V");
                    Assert.AreEqual(vertex3.GetField<string>("Foo"), "foo string value3");
                    Assert.AreEqual(vertex3.GetField<int>("Bar"), 347899);
                     
                    OEdge edge1 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("Foo", "foo string value3")
                        .Set("Bar", 123)
                        .Run<OEdge>();

                    Assert.IsTrue(!string.IsNullOrEmpty(edge1.ORID.ToString()));
                    Assert.AreEqual(edge1.Label, "E");
                    Assert.AreEqual(edge1.OClassName, "E");
                    Assert.AreEqual(edge1.InV, vertex2.ORID);
                    Assert.AreEqual(edge1.OutV, vertex1.ORID);
                    Assert.AreEqual(edge1.GetField<string>("Foo"), "foo string value3");
                    Assert.AreEqual(edge1.GetField<int>("Bar"), 123);

                    OEdge edge2 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex3)
                        .Set("Foo", "foo string value4")
                        .Set("Bar", 245)
                        .Run<OEdge>();

                    Assert.IsTrue(!string.IsNullOrEmpty(edge2.ORID.ToString()));
                    Assert.AreEqual(edge2.Label, "E");
                    Assert.AreEqual(edge2.OClassName, "E");
                    Assert.AreEqual(edge2.InV, vertex3.ORID);
                    Assert.AreEqual(edge2.OutV, vertex1.ORID);
                    Assert.AreEqual(edge2.GetField<string>("Foo"), "foo string value4");
                    Assert.AreEqual(edge2.GetField<int>("Bar"), 245);

                    OVertex loadedVertex1 = database
                        .Select()
                        .From(vertex1)
                        .ToList<OVertex>().First();

                    Assert.IsTrue(!string.IsNullOrEmpty(loadedVertex1.ORID.ToString()));
                    Assert.AreEqual(loadedVertex1.OClassName, "V");
                    Assert.AreEqual(loadedVertex1.InE.Count, 0);
                    Assert.AreEqual(loadedVertex1.OutE.Count, 2);
                    Assert.IsTrue(loadedVertex1.OutE.Contains(edge1.ORID));
                    Assert.IsTrue(loadedVertex1.OutE.Contains(edge2.ORID));
                    Assert.AreEqual(loadedVertex1.GetField<string>("Foo"), vertex1.GetField<string>("Foo"));
                    Assert.AreEqual(loadedVertex1.GetField<int>("Bar"), vertex1.GetField<int>("Bar"));

                    OVertex loadedVertex2 = database
                        .Select()
                        .From(vertex2)
                        .ToList<OVertex>().First();

                    Assert.IsTrue(!string.IsNullOrEmpty(loadedVertex2.ORID.ToString()));
                    Assert.AreEqual(loadedVertex2.OClassName, "V");
                    Assert.AreEqual(loadedVertex2.OutE.Count, 0);
                    Assert.AreEqual(loadedVertex2.InE.Count, 1);
                    Assert.IsTrue(loadedVertex2.InE.Contains(edge1.ORID));
                    Assert.AreEqual(loadedVertex2.GetField<string>("Foo"), vertex2.GetField<string>("Foo"));
                    Assert.AreEqual(loadedVertex2.GetField<int>("Bar"), vertex2.GetField<int>("Bar"));
                }
            }
        }
    }
}
