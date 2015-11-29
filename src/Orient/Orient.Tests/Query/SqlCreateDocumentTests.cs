using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlCreateDocumentTests
    {
        [Fact]
        public void ShouldCreateDocumentFromDocument()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document = new ODocument();
                    document.OClassName = "TestClass";
                    document
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    ODocument createdDocument = database
                        .Create.Document(document)
                        .Run();

                    Assert.NotNull(createdDocument.ORID);
                    Assert.Equal("TestClass", createdDocument.OClassName);
                    Assert.Equal(document.GetField<string>("foo"), createdDocument.GetField<string>("foo"));
                    Assert.Equal(document.GetField<int>("bar"), createdDocument.GetField<int>("bar"));
                }
            }
        }

        [Fact]
        public void ShouldCreateDocumentFromObject()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestProfileClass>()
                        .Run();

                    TestProfileClass profile = new TestProfileClass();
                    profile.Name = "Johny";
                    profile.Surname = "Bravo";

                    TestProfileClass createdObject = database
                        .Create.Document(profile)
                        .Run<TestProfileClass>();

                    Assert.NotNull(createdObject.ORID);
                    Assert.Equal(typeof(TestProfileClass).Name, createdObject.OClassName);
                    Assert.Equal(profile.Name, createdObject.Name);
                    Assert.Equal(profile.Surname, createdObject.Surname);
                }
            }
        }

        [Fact]
        public void ShouldCreateDocumentClassSet()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document = new ODocument();
                    document.OClassName = "TestClass";
                    document
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    ODocument createdDocument = database
                        .Create.Document("TestClass")
                        .Set(document)
                        .Run();

                    Assert.NotNull(createdDocument.ORID);
                    Assert.Equal("TestClass", createdDocument.OClassName);
                    Assert.Equal(document.GetField<string>("foo"), createdDocument.GetField<string>("foo"));
                    Assert.Equal(document.GetField<int>("bar"), createdDocument.GetField<int>("bar"));
                }
            }
        }
    }
}
