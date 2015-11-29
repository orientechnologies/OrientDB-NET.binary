using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlCreateVertexTests
    {
        [Fact]
        public void ShouldCreateVertexSet()
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

                    OVertex createdVertex = database
                        .Create.Vertex("TestVertexClass")
                        .Set("foo", "foo string value")
                        .Set("bar", 12345)
                        .Run();

                    Assert.NotNull(createdVertex.ORID);
                    Assert.Equal("TestVertexClass", createdVertex.OClassName);
                    Assert.Equal("foo string value", createdVertex.GetField<string>("foo"));
                    Assert.Equal(12345, createdVertex.GetField<int>("bar"));
                }
            }
        }

        [Fact]
        public void ShouldCreateVertexFromDocument()
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

                    ODocument document = new ODocument();
                    document.OClassName = "TestVertexClass";
                    document
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    OVertex createdVertex = database
                        .Create.Vertex(document)
                        .Run();

                    Assert.NotNull(createdVertex.ORID);
                    Assert.Equal("TestVertexClass", createdVertex.OClassName);
                    Assert.Equal(document.GetField<string>("foo"), createdVertex.GetField<string>("foo"));
                    Assert.Equal(document.GetField<int>("bar"), createdVertex.GetField<int>("bar"));
                }
            }
        }

        [Fact]
        public void ShouldCreateVertexFromOVertex()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    OVertex vertex = new OVertex();
                    vertex
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    OVertex createdVertex = database
                        .Create.Vertex(vertex)
                        .Run();

                    Assert.NotNull(createdVertex.ORID);
                    Assert.Equal("V", createdVertex.OClassName);
                    Assert.Equal(vertex.GetField<string>("foo"), createdVertex.GetField<string>("foo"));
                    Assert.Equal(vertex.GetField<int>("bar"), createdVertex.GetField<int>("bar"));
                }
            }
        }

        [Fact]
        public void ShouldCreateVertexFromObject()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestProfileClass>()
                        .Extends<OVertex>()
                        .Run();

                    TestProfileClass profile = new TestProfileClass();
                    profile.Name = "Johny";
                    profile.Surname = "Bravo";

                    TestProfileClass createdVertex = database
                        .Create.Vertex(profile)
                        .Run<TestProfileClass>();

                    Assert.NotNull(createdVertex.ORID);
                    Assert.Equal(typeof(TestProfileClass).Name, createdVertex.OClassName);
                    Assert.Equal(profile.Name, createdVertex.Name);
                    Assert.Equal(profile.Surname, createdVertex.Surname);
                }
            }
        }
    }
}
