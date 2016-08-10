using NUnit.Framework;
using Orient.Client;


namespace Orient.Nunit.Test.Query
{
    [TestFixture]
    public class GremlinTests
    {
        [Test]
        public void ShouldExecuteSimpleGremlinQuery()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

                    OCommandResult documents = database.Gremlin("g.V");
                }
            }
        }
    }
}
