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
	public void ShouldNotThrowExceptionWhenCreatingVerticesWithNonASCIIChars(){
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

                    OEdge edge = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("Foo", "foo string value3")
                        .Set("Bar", 123)
                        .Run<OEdge>();

                    Assert.IsTrue(!string.IsNullOrEmpty(edge.ORID.ToString()));
                    Assert.AreEqual(edge.Label, "E");
                    Assert.AreEqual(edge.OClassName, "E");
                    Assert.AreEqual(edge.InV, vertex2.ORID);
                    Assert.AreEqual(edge.OutV, vertex1.ORID);
                    Assert.AreEqual(edge.GetField<string>("Foo"), "foo string value3");
                    Assert.AreEqual(edge.GetField<int>("Bar"), 123);

                    OVertex loadedVertex1 = database
                        .Select()
                        .From(vertex1)
                        .ToList<OVertex>().First();

                    Assert.IsTrue(!string.IsNullOrEmpty(loadedVertex1.ORID.ToString()));
                    Assert.AreEqual(loadedVertex1.OClassName, "V");
                    Assert.AreEqual(loadedVertex1.InE.Count, 0);
                    Assert.AreEqual(loadedVertex1.OutE.Count, 1);
                    Assert.IsTrue(loadedVertex1.OutE.Contains(edge.ORID));
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
                    Assert.IsTrue(loadedVertex2.InE.Contains(edge.ORID));
                    Assert.AreEqual(loadedVertex2.GetField<string>("Foo"), vertex2.GetField<string>("Foo"));
                    Assert.AreEqual(loadedVertex2.GetField<int>("Bar"), vertex2.GetField<int>("Bar"));
                }
            }
        }
    }
}
