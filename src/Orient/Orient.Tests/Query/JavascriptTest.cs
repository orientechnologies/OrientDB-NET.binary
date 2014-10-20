using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class JavascriptTest
    {
        [TestMethod]
        [TestCategory("Script")]
        public void ShouldExecuteSimpleJavascriptQuery()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
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

                    List<ODocument> documents = database.JavaScript("db.command('select from V');");
                    Assert.AreEqual(2, documents.Count);
                    var loadedVertex1 = documents.Find(d => d.ORID.Equals(vertex1.ORID));
                    var loadedVertex2 = documents.Find(d => d.ORID.Equals(vertex2.ORID));
                    Assert.IsNotNull(loadedVertex1);
                    Assert.IsNotNull(loadedVertex2);

                    Assert.AreEqual(vertex1.GetField<string>("Foo"), loadedVertex1.GetField<string>("Foo"));
                    Assert.AreEqual(vertex1.GetField<int>("Bar"), loadedVertex1.GetField<int>("Bar"));
                    Assert.AreEqual(vertex2.GetField<string>("Foo"), loadedVertex2.GetField<string>("Foo"));
                    Assert.AreEqual(vertex2.GetField<int>("Bar"), loadedVertex2.GetField<int>("Bar"));

                    Assert.AreEqual(1, loadedVertex1.GetField<HashSet<ORID>>("out_").Count);
                    Assert.IsTrue(loadedVertex1.GetField<HashSet<ORID>>("out_").Contains(edge1.ORID));
                    Assert.IsTrue(loadedVertex2.GetField<HashSet<ORID>>("in_").Contains(edge1.ORID));
                }
            }
        }
    }
}
