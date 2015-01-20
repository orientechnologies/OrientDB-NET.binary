using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class SqlCreateDocumentTests
    {
        [Test]
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

                    Assert.IsNotNull(createdDocument.ORID);
                    Assert.AreEqual("TestClass", createdDocument.OClassName);
                    Assert.AreEqual(document.GetField<string>("foo"), createdDocument.GetField<string>("foo"));
                    Assert.AreEqual(document.GetField<int>("bar"), createdDocument.GetField<int>("bar"));
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdObject.ORID);
                    Assert.AreEqual(typeof(TestProfileClass).Name, createdObject.OClassName);
                    Assert.AreEqual(profile.Name, createdObject.Name);
                    Assert.AreEqual(profile.Surname, createdObject.Surname);
                }
            }
        }

        [Test]
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

                    Assert.IsNotNull(createdDocument.ORID);
                    Assert.AreEqual("TestClass", createdDocument.OClassName);
                    Assert.AreEqual(document.GetField<string>("foo"), createdDocument.GetField<string>("foo"));
                    Assert.AreEqual(document.GetField<int>("bar"), createdDocument.GetField<int>("bar"));
                }
            }
        }
    }
}
