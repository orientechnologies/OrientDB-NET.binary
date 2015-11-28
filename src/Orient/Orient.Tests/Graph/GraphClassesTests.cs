using System;
using System.Linq;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Graph
{
    
    public class GraphClassesTests
    {
        [Fact]
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
        [Fact]
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

                    Assert.True(!string.IsNullOrEmpty(vertex1.ORID.ToString()));
                    Assert.Equal("V", vertex1.OClassName);
                    Assert.Equal("foo string value1", vertex1.GetField<string>("Foo"));
                    Assert.Equal(12345, vertex1.GetField<int>("Bar"));

                    OVertex vertex2 = database
                        .Create.Vertex<OVertex>()
                        .Set("Foo", "foo string value2")
                        .Set("Bar", 54321)
                        .Run<OVertex>();

                    Assert.True(!string.IsNullOrEmpty(vertex2.ORID.ToString()));
                    Assert.Equal("V", vertex2.OClassName);
                    Assert.Equal("foo string value2", vertex2.GetField<string>("Foo"));
                    Assert.Equal(54321, vertex2.GetField<int>("Bar"));

                    OVertex vertex3 = database
                        .Create.Vertex<OVertex>()
                        .Set("Foo", "foo string value3")
                        .Set("Bar", 347899)
                        .Run<OVertex>();

                    Assert.True(!string.IsNullOrEmpty(vertex3.ORID.ToString()));
                    Assert.Equal("V", vertex3.OClassName);
                    Assert.Equal("foo string value3", vertex3.GetField<string>("Foo"));
                    Assert.Equal(347899, vertex3.GetField<int>("Bar"));

                    OEdge edge1 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex2)
                        .Set("Foo", "foo string value3")
                        .Set("Bar", 123)
                        .Run<OEdge>();

                    Assert.True(!string.IsNullOrEmpty(edge1.ORID.ToString()));
                    Assert.Equal("E", edge1.Label);
                    Assert.Equal("E", edge1.OClassName);
                    Assert.Equal(vertex2.ORID, edge1.InV);
                    Assert.Equal(vertex1.ORID, edge1.OutV);
                    Assert.Equal("foo string value3", edge1.GetField<string>("Foo"));
                    Assert.Equal(123, edge1.GetField<int>("Bar"));

                    OEdge edge2 = database
                        .Create.Edge<OEdge>()
                        .From(vertex1)
                        .To(vertex3)
                        .Set("Foo", "foo string value4")
                        .Set("Bar", 245)
                        .Run<OEdge>();

                    Assert.True(!string.IsNullOrEmpty(edge2.ORID.ToString()));
                    Assert.Equal("E", edge2.Label);
                    Assert.Equal("E", edge2.OClassName);
                    Assert.Equal(vertex3.ORID, edge2.InV);
                    Assert.Equal(vertex1.ORID, edge2.OutV);
                    Assert.Equal("foo string value4", edge2.GetField<string>("Foo"));
                    Assert.Equal(245, edge2.GetField<int>("Bar"));

                    OVertex loadedVertex1 = database
                        .Select()
                        .From(vertex1)
                        .ToList<OVertex>().First();

                    Assert.True(!string.IsNullOrEmpty(loadedVertex1.ORID.ToString()));
                    Assert.Equal("V", loadedVertex1.OClassName);
                    Assert.Equal(0, loadedVertex1.InE.Count);
                    Assert.Equal(2, loadedVertex1.OutE.Count);
                    Assert.True(loadedVertex1.OutE.Contains(edge1.ORID));
                    Assert.True(loadedVertex1.OutE.Contains(edge2.ORID));
                    Assert.Equal(vertex1.GetField<string>("Foo"), loadedVertex1.GetField<string>("Foo"));
                    Assert.Equal(vertex1.GetField<int>("Bar"), loadedVertex1.GetField<int>("Bar"));

                    OVertex loadedVertex2 = database
                        .Select()
                        .From(vertex2)
                        .ToList<OVertex>().First();

                    Assert.True(!string.IsNullOrEmpty(loadedVertex2.ORID.ToString()));
                    Assert.Equal("V", loadedVertex2.OClassName);
                    Assert.Equal(0, loadedVertex2.OutE.Count);
                    Assert.Equal(1, loadedVertex2.InE.Count);
                    Assert.True(loadedVertex2.InE.Contains(edge1.ORID));
                    Assert.Equal(vertex2.GetField<string>("Foo"), loadedVertex2.GetField<string>("Foo"));
                    Assert.Equal(vertex2.GetField<int>("Bar"), loadedVertex2.GetField<int>("Bar"));
                }
            }
        }
    }
}
