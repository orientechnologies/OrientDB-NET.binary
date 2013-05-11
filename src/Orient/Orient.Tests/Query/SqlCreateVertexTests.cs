using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlCreateVertexTests
    {
        [TestMethod]
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

                    Assert.IsTrue(createdVertex.ORID != null);
                    Assert.AreEqual(createdVertex.OClassName, "TestVertexClass");
                    Assert.AreEqual(createdVertex.GetField<string>("foo"), "foo string value");
                    Assert.AreEqual(createdVertex.GetField<int>("bar"), 12345);
                }
            }
        }

        [TestMethod]
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

                    Assert.IsTrue(createdVertex.ORID != null);
                    Assert.AreEqual(createdVertex.OClassName, "TestVertexClass");
                    Assert.AreEqual(createdVertex.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(createdVertex.GetField<int>("bar"), document.GetField<int>("bar"));
                }
            }
        }

        [TestMethod]
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

                    Assert.IsTrue(createdVertex.ORID != null);
                    Assert.AreEqual(createdVertex.OClassName, "V");
                    Assert.AreEqual(createdVertex.GetField<string>("foo"), vertex.GetField<string>("foo"));
                    Assert.AreEqual(createdVertex.GetField<int>("bar"), vertex.GetField<int>("bar"));
                }
            }
        }

        [TestMethod]
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

                    Assert.IsTrue(createdVertex.ORID != null);
                    Assert.AreEqual(createdVertex.OClassName, typeof(TestProfileClass).Name);
                    Assert.AreEqual(createdVertex.Name, profile.Name);
                    Assert.AreEqual(createdVertex.Surname, profile.Surname);
                }
            }
        }
    }
}
