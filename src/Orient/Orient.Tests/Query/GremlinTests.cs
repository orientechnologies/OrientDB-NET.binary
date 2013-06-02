using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class GremlinTests
    {
        [TestMethod]
        public void ShouldExecuteSimpleGremlinQuery()
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

                    List<ODocument> documents = database.Gremlin("g.V");
                }
            }
        }
    }
}
