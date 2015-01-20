using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class SqlCreateVertexTests
    {
        [Test]
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

                    Assert.IsNotNull(createdVertex.ORID);
                    Assert.AreEqual("TestVertexClass", createdVertex.OClassName);
                    Assert.AreEqual("foo string value", createdVertex.GetField<string>("foo"));
                    Assert.AreEqual(12345, createdVertex.GetField<int>("bar"));
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdVertex.ORID);
                    Assert.AreEqual("TestVertexClass", createdVertex.OClassName);
                    Assert.AreEqual(document.GetField<string>("foo"), createdVertex.GetField<string>("foo"));
                    Assert.AreEqual(document.GetField<int>("bar"), createdVertex.GetField<int>("bar"));
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdVertex.ORID);
                    Assert.AreEqual("V", createdVertex.OClassName);
                    Assert.AreEqual(vertex.GetField<string>("foo"), createdVertex.GetField<string>("foo"));
                    Assert.AreEqual(vertex.GetField<int>("bar"), createdVertex.GetField<int>("bar"));
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdVertex.ORID);
                    Assert.AreEqual(typeof(TestProfileClass).Name, createdVertex.OClassName);
                    Assert.AreEqual(profile.Name, createdVertex.Name);
                    Assert.AreEqual(profile.Surname, createdVertex.Surname);
                }
            }
        }
    }
}
