using System;
using System.Collections.Generic;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlInsertTests
    {
        [Fact]
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

                    Assert.True(insertedDocument.ORID != null);
                    Assert.Equal(insertedDocument.OClassName, "TestClass");
                    Assert.Equal(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.Equal(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));
                }
            }
        }

        [Fact]
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

                    Assert.True(insertedDocument.ORID != null);
                    Assert.Equal(insertedDocument.OClassName, typeof(TestProfileClass).Name);
                    Assert.Equal(insertedDocument.Name, profile.Name);
                    Assert.Equal(insertedDocument.Surname, profile.Surname);
                }
            }
        }

        [Fact]
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

                    Assert.True(insertedDocument.ORID != null);
                    Assert.Equal(insertedDocument.OClassName, "TestClass");
                    Assert.Equal(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.Equal(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));


                }
            }
        }

        [Fact]
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

                    Assert.True(insertedDocument.ORID != null);
                    Assert.Equal(insertedDocument.OClassName, "TestClass");
                    Assert.Equal(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
                    Assert.Equal(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));
                }
            }
        }

        [Fact]
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

                    Assert.True(insertedDocument.ORID != null);
                    Assert.Equal(insertedDocument.OClassName, "TestClass");
                    Assert.Equal(insertedDocument.GetField<string>("foo"), "foo string value");
                    Assert.Equal(insertedDocument.GetField<int>("bar"), 12345);

         
                }
            }
        }
    }
}
