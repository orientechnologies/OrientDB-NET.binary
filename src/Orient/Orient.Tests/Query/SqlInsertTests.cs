using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlInsertTests
    {
        [TestMethod]
        public void ShouldInsertDocument()
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

        [TestMethod]
        public void ShouldInsertObject()
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

        [TestMethod]
        public void ShouldInsertDocumentInto()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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

        [TestMethod]
        public void ShouldInsertDocumentIntoCluster()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    database
                        .Create.Cluster("TestCluster", OClusterType.Physical)
                        .Run();

                    database.Command("alter class TestClass addcluster TestCluster");

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

        [TestMethod]
        public void ShouldInsertIntoClusterSet()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    database
                        .Create.Cluster("TestCluster", OClusterType.Physical)
                        .Run();
                    
                    database.Command("alter class TestClass addcluster TestCluster");

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
