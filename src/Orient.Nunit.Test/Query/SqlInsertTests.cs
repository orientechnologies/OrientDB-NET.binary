using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlInsertTests
    {
        [Test]
        public void ShouldInsertDocument()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
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

                    ODocument insertedDocument = database
                        .Insert(document)
                        .Run();

                    Assert.IsTrue(insertedDocument.ORID != null);
                    Assert.AreEqual(insertedDocument.OClassName, "TestClass");
                    Assert.AreEqual(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));
                }
            }
        }

        [Test]
        public void ShouldInsertObject()
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

                    TestProfileClass insertedDocument = database
                        .Insert(profile)
                        .Run<TestProfileClass>();

                    Assert.IsTrue(insertedDocument.ORID != null);
                    Assert.AreEqual(insertedDocument.OClassName, typeof(TestProfileClass).Name);
                    Assert.AreEqual(insertedDocument.Name, profile.Name);
                    Assert.AreEqual(insertedDocument.Surname, profile.Surname);
                }
            }
        }

        [Test]
        public void ShouldInsertDocumentInto()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document = new ODocument()
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    ODocument insertedDocument = database
                        .Insert(document)
                        .Into("TestClass")
                        .Run();

                    Assert.IsTrue(insertedDocument.ORID != null);
                    Assert.AreEqual(insertedDocument.OClassName, "TestClass");
                    Assert.AreEqual(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));


                }
            }
        }

        [Ignore("Broken Tests as at Github 27594c0114cd9489b69c84fe4896a9d6c6d01b19")]
        [Test]
        public void ShouldInsertDocumentIntoCluster()
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
                        .Create.Cluster("TestCluster", OClusterType.Physical)
                        .Run();

                    ODocument document = new ODocument()
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    ODocument insertedDocument = database
                        .Insert(document)
                        .Into("TestClass")
                        .Cluster("TestCluster")
                        .Run();

                    Assert.IsTrue(insertedDocument.ORID != null);
                    Assert.AreEqual(insertedDocument.OClassName, "TestClass");
                    Assert.AreEqual(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.AreEqual(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));
                }
            }
        }
        [Ignore("Broken Tests as at Github 27594c0114cd9489b69c84fe4896a9d6c6d01b19")]
        [Test]
        public void ShouldInsertIntoClusterSet()
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
                        .Create.Cluster("TestCluster", OClusterType.Physical)
                        .Run();

                    ODocument insertedDocument = database
                        .Insert()
                        .Into("TestClass")
                        .Cluster("TestCluster")
                        .Set("foo", "foo string value")
                        .Set("bar", 12345)
                        .Run();

                    Assert.IsTrue(insertedDocument.ORID != null);
                    Assert.AreEqual(insertedDocument.OClassName, "TestClass");
                    Assert.AreEqual(insertedDocument.GetField<string>("foo"), "foo string value");
                    Assert.AreEqual(insertedDocument.GetField<int>("bar"), 12345);

         
                }
            }
        }
    }
}
