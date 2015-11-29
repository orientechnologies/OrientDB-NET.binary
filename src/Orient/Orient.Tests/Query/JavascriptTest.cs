using System;
using System.Collections.Generic;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class JavascriptTest
    {
        [Fact]
        [Trait("type", "Script")]
        public void ShouldExecuteSimpleJavascriptQuery()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                OVertex vertex1 = database
                    .Create.Vertex<OVertex>()
                    .Set("Foo", "foo string value 1")
                    .Set("Bar", 12345)
                    .Run();

                OVertex vertex2 = database
                    .Create.Vertex<OVertex>()
                    .Set("Foo", "foo string value 2")
                    .Set("Bar", 54321)
                    .Run();

                OEdge edge1 = database
                    .Create.Edge<OEdge>()
                    .From(vertex1)
                    .To(vertex2)
                    .Set("Foo", "foo string value 3")
                    .Set("Bar", 123)
                    .Run();

                List<ODocument> documents = database
                    .JavaScript("db.command('select from V');")
                    .Run()
                    .ToList();

                Assert.Equal(2, documents.Count);
                var loadedVertex1 = documents.Find(d => d.ORID.Equals(vertex1.ORID));
                var loadedVertex2 = documents.Find(d => d.ORID.Equals(vertex2.ORID));
                Assert.NotNull(loadedVertex1);
                Assert.NotNull(loadedVertex2);

                Assert.Equal(vertex1.GetField<string>("Foo"), loadedVertex1.GetField<string>("Foo"));
                Assert.Equal(vertex1.GetField<int>("Bar"), loadedVertex1.GetField<int>("Bar"));
                Assert.Equal(vertex2.GetField<string>("Foo"), loadedVertex2.GetField<string>("Foo"));
                Assert.Equal(vertex2.GetField<int>("Bar"), loadedVertex2.GetField<int>("Bar"));

                Assert.Equal(1, loadedVertex1.GetField<HashSet<ORID>>("out_").Count);
                Assert.True(loadedVertex1.GetField<HashSet<ORID>>("out_").Contains(edge1.ORID));
                Assert.True(loadedVertex2.GetField<HashSet<ORID>>("in_").Contains(edge1.ORID));
            }
        }

        [Fact]
        [Trait("type", "Script")]
        public void ShouldExecuteSimpleFunction()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database
                    .Command("create function sum 'return parseInt(a)+parseInt(b);' parameters [a , b] language javascript");
                var result = database
                    .JavaScript("sum(a,b);")
                    .Set("a",3)
                    .Set("b",5)
                    .Run()
                    .ToSingle();
                
                Assert.NotNull(result);
                Assert.Equal(1, result.Count);
                var actual = result.GetField<string>("value");
                if (actual != "8") // 8 seems to come back from this call - maybe depends on exact version of OrientDB server?... Anyway, it's a good enough result
                {
                    Assert.Equal("8.0d", actual);
                }
            }
        }
    }
}
