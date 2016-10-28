using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlDeleteDocumentTests
    {
        [Test]
        public void ShouldDeleteDocumentFromDocumentOClassName()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    database
                        .Create.Document("TestClass")
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    database
                        .Create.Document("TestClass")
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument document = new ODocument();
                    document.OClassName = "TestClass";

                    int documentsDeleted = database
                        .Delete.Document(document)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 2);
                }
            }
        }

        [Test]
        public void ShouldDeleteDocumentFromObjectOClassName()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class<TestProfileClass>()
                        .Run();

                    TestProfileClass profile = new TestProfileClass();
                    profile.Name = "Johny";
                    profile.Surname = "Bravo";

                    database
                        .Create.Document(profile)
                        .Run();

                    database
                        .Create.Document(profile)
                        .Run();

                    int documentsDeleted = database
                        .Delete.Document(profile)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 2);
                }
            }
        }

        [Test]
        public void ShouldDeleteDocumentFromDocumentOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document1 = database
                        .Create.Document("TestClass")
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    ODocument document2 = database
                        .Create.Document("TestClass")
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument document = new ODocument();
                    document.OClassName = "TestClass";

                    int documentsDeleted = database
                        .Delete.Document(document2)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [Test]
        public void ShouldDeleteDocumentFromObjectOrid()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class<TestProfileClass>()
                        .Run();

                    TestProfileClass profile = new TestProfileClass();
                    profile.Name = "Johny";
                    profile.Surname = "Bravo";

                    TestProfileClass document1 = database
                        .Create.Document(profile)
                        .Run<TestProfileClass>();

                    TestProfileClass document2 = database
                        .Create.Document(profile)
                        .Run<TestProfileClass>();

                    int documentsDeleted = database
                        .Delete.Document(document2)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }

        [Test]
        public void ShouldDeleteDocumentClassWhereQuery()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    database
                        .Create.Document("TestClass")
                        .Set("foo", "foo string value1")
                        .Set("bar", 12345)
                        .Run();

                    database
                        .Create.Document("TestClass")
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument document = new ODocument();
                    document.OClassName = "TestClass";

                    int documentsDeleted = database
                        .Delete.Document("TestClass")
                        .Where("bar").Equals(12345)
                        .Run();

                    Assert.AreEqual(documentsDeleted, 1);
                }
            }
        }
    }
}
