using System;
using System.Collections.Generic;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlUpdateTests
    {
        [Fact]
        public void ShouldUpdateClassFromDocument()
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

                    database
                        .Insert(document)
                        .Run();

                    database
                        .Insert(document)
                        .Run();

                    document
                        .SetField("bar", 54321)
                        .SetField("baz", "new baz value");

                    int documentsUpdated = database
                        .Update(document)
                        .Run();

                    Assert.Equal(documentsUpdated, 2);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .ToList();

                    Assert.Equal(documents.Count, 2);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.True(documents[i].ORID != null);
                        Assert.Equal(documents[i].OClassName, document.OClassName);
                        Assert.Equal(documents[i].GetField<string>("foo"), document.GetField<string>("foo"));
                        Assert.Equal(documents[i].GetField<int>("bar"), document.GetField<int>("bar"));
                        Assert.Equal(documents[i].GetField<string>("baz"), document.GetField<string>("baz"));
                    }
                }
            }
        }

        [Fact]
        public void ShouldUpdateClassFromObject()
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

                    database
                        .Insert(profile)
                        .Run();

                    database
                        .Insert(profile)
                        .Run();

                    profile.Name = "Julia";
                    profile.Surname = "Bravo";

                    int documentsUpdated = database
                        .Update(profile)
                        .Run();

                    Assert.Equal(documentsUpdated, 2);

                    List<TestProfileClass> documents = database
                        .Select()
                        .From<TestProfileClass>()
                        .ToList<TestProfileClass>();

                    Assert.Equal(documents.Count, 2);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.True(documents[i].ORID != null);
                        Assert.Equal(documents[i].OClassName, typeof(TestProfileClass).Name);
                        Assert.Equal(documents[i].Name, profile.Name);
                        Assert.Equal(documents[i].Surname, profile.Surname);
                    }
                }
            }
        }

        [Fact]
        public void ShouldUpdateClass()
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
                    document
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    database
                        .Insert(document)
                        .Into("TestClass")
                        .Run();

                    database
                        .Insert(document)
                        .Into("TestClass")
                        .Run();

                    document
                        .SetField("bar", 54321)
                        .SetField("baz", "new baz value");

                    int documentsUpdated = database
                        .Update(document)
                        .Class("TestClass")
                        .Run();

                    Assert.Equal(documentsUpdated, 2);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .ToList();

                    Assert.Equal(documents.Count, 2);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.True(documents[i].ORID != null);
                        Assert.Equal(documents[i].OClassName, "TestClass");
                        Assert.Equal(documents[i].GetField<string>("foo"), document.GetField<string>("foo"));
                        Assert.Equal(documents[i].GetField<int>("bar"), document.GetField<int>("bar"));
                        Assert.Equal(documents[i].GetField<string>("baz"), document.GetField<string>("baz"));
                    }
                }
            }
        }

        
        [Fact]
        public void ShouldUpdateCluster()
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

                    ODocument document = new ODocument();
                    document.OClassName = "TestClass";
                    document
                        .SetField("foo", "foo string value")
                        .SetField("bar", 12345);

                    database
                        .Insert(document)
                        .Cluster("TestCluster")
                        .Run();

                    database
                        .Insert(document)
                        .Cluster("TestCluster")
                        .Run();

                    document
                        .SetField("bar", 54321)
                        .SetField("baz", "new baz value");

                    int documentsUpdated = database
                        .Update(document)
                        .Cluster("TestCluster")
                        .Run();

                    Assert.Equal(documentsUpdated, 2);

                    List<ODocument> documents = database
                        .Select()
                        .From("cluster:TestCluster")
                        .ToList();

                    Assert.Equal(documents.Count, 2);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.True(documents[i].ORID != null);
                        Assert.Equal(documents[i].OClassName, document.OClassName);
                        Assert.Equal(documents[i].GetField<string>("foo"), document.GetField<string>("foo"));
                        Assert.Equal(documents[i].GetField<int>("bar"), document.GetField<int>("bar"));
                        Assert.Equal(documents[i].GetField<string>("baz"), document.GetField<string>("baz"));
                    }
                }
            }
        }

        [Fact]
        public void ShouldUpdateRecordFromDocument()
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

                    ODocument document1 = database
                        .Insert(document)
                        .Run();

                    ODocument document2 = database
                        .Insert(document)
                        .Run();

                    document2
                        .SetField("bar", 54321)
                        .SetField("baz", "new baz value");

                    int documentsUpdated = database
                        .Update(document2)
                        .Run();

                    Assert.Equal(documentsUpdated, 1);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .Where("bar").Equals(54321)
                        .ToList();

                    Assert.Equal(documents.Count, 1);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.Equal(documents[i].ORID, document2.ORID);
                        Assert.Equal(documents[i].OClassName, document2.OClassName);
                        Assert.Equal(documents[i].GetField<string>("foo"), document2.GetField<string>("foo"));
                        Assert.Equal(documents[i].GetField<int>("bar"), document2.GetField<int>("bar"));
                        Assert.Equal(documents[i].GetField<string>("baz"), document2.GetField<string>("baz"));
                    }
                }
            }
        }

        [Fact]
        public void ShouldUpdateOridSet()
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

                    ODocument document1 = database
                        .Insert(document)
                        .Run();

                    ODocument document2 = database
                        .Insert(document)
                        .Run();

                    document2
                        .SetField("bar", 54321)
                        .SetField("baz", "new baz value");

                    int documentsUpdated = database
                        .Update(document2.ORID)
                        .Set(document2)
                        .Run();

                    Assert.Equal(documentsUpdated, 1);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .Where("bar").Equals(54321)
                        .ToList();

                    Assert.Equal(documents.Count, 1);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.Equal(documents[i].ORID, document2.ORID);
                        Assert.Equal(documents[i].OClassName, document2.OClassName);
                        Assert.Equal(documents[i].GetField<string>("foo"), document2.GetField<string>("foo"));
                        Assert.Equal(documents[i].GetField<int>("bar"), document2.GetField<int>("bar"));
                        Assert.Equal(documents[i].GetField<string>("baz"), document2.GetField<string>("baz"));
                    }
                }
            }
        }

        [Fact]
        public void ShouldUpdateRecord()
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

                    ODocument document1 = database
                        .Insert(document)
                        .Run();

                    ODocument document2 = database
                        .Insert(document)
                        .Run();

                    ODocument docToUpdate = new ODocument()
                        .SetField("bar", 54321)
                        .SetField("baz", "new baz value");

                    int documentsUpdated = database
                        .Update(docToUpdate)
                        .Record(document2.ORID)
                        .Run();

                    Assert.Equal(documentsUpdated, 1);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .Where("bar").Equals(54321)
                        .ToList();

                    Assert.Equal(documents.Count, 1);

                    Assert.Equal(documents[0].ORID, document2.ORID);
                    Assert.Equal(documents[0].OClassName, document2.OClassName);
                    Assert.Equal(documents[0].GetField<string>("foo"), document2.GetField<string>("foo"));
                    Assert.Equal(documents[0].GetField<int>("bar"), docToUpdate.GetField<int>("bar"));
                    Assert.Equal(documents[0].GetField<string>("baz"), docToUpdate.GetField<string>("baz"));
                }
            }
        }

        [Fact]
        public void ShouldUpdateRecordSet()
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

                    ODocument document1 = database
                        .Insert(document)
                        .Run();

                    ODocument document2 = database
                        .Insert(document)
                        .Run();

                    int documentsUpdated = database
                        .Update(document2.ORID)
                        .Set("bar", 54321)
                        .Set("baz", "new baz value")
                        .Run();

                    Assert.Equal(documentsUpdated, 1);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .Where("bar").Equals(54321)
                        .ToList();

                    Assert.Equal(documents.Count, 1);

                    Assert.Equal(documents[0].ORID, document2.ORID);
                    Assert.Equal(documents[0].OClassName, document2.OClassName);
                    Assert.Equal(documents[0].GetField<string>("foo"), document2.GetField<string>("foo"));
                    Assert.Equal(documents[0].GetField<int>("bar"), 54321);
                    Assert.Equal(documents[0].GetField<string>("baz"), "new baz value");
                }
            }
        }

        [Fact]
        public void ShouldUpdateWhere()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document1 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", "foo string value")
                        .Set("bar", 11111)
                        .Run();

                    ODocument document2 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", "foo string value")
                        .Set("bar", 12345)
                        .Run();

                    int documentsUpdated = database
                        .Update()
                        .Class("TestClass")
                        .Set("bar", 54321)
                        .Set("baz", "new baz value")
                        .Where("bar").Equals(12345)
                        .Run();

                    Assert.Equal(documentsUpdated, 1);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .Where("bar").Equals(54321)
                        .ToList();

                    Assert.Equal(documents.Count, 1);

                    Assert.Equal(documents[0].ORID, document2.ORID);
                    Assert.Equal(documents[0].OClassName, document2.OClassName);
                    Assert.Equal(documents[0].GetField<string>("foo"), document2.GetField<string>("foo"));
                    Assert.Equal(documents[0].GetField<int>("bar"), 54321);
                    Assert.Equal(documents[0].GetField<string>("baz"), "new baz value");
                }
            }
        }

        [Fact]
        public void ShouldUpdateAddCollectionItem()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document1 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", new List<string>() { "foo1", "foo2" })
                        .Set("bar", 11111)
                        .Run();

                    ODocument document2 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", new List<string>() { "foo1", "foo2" })
                        .Set("bar", 12345)
                        .Run();

                    int documentsUpdated = database
                        .Update(document2)
                        .Add("foo", "foo3")
                        .Run();

                    Assert.Equal(documentsUpdated, 1);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .Where("bar").Equals(12345)
                        .ToList();

                    Assert.Equal(documents.Count, 1);

                    Assert.Equal(documents[0].ORID, document2.ORID);
                    Assert.Equal(documents[0].OClassName, document2.OClassName);

                    List<string> foos = new List<string>() { "foo1", "foo2", "foo3" };

                    Assert.Equal(documents[0].GetField<List<string>>("foo").Count, foos.Count);
                    Assert.Equal(documents[0].GetField<List<string>>("foo")[0], foos[0]);
                    Assert.Equal(documents[0].GetField<List<string>>("foo")[1], foos[1]);
                    Assert.Equal(documents[0].GetField<List<string>>("foo")[2], foos[2]);

                    Assert.Equal(documents[0].GetField<int>("bar"), document2.GetField<int>("bar"));
                }
            }
        }

        [Fact]
        public void ShouldUpdateRemoveCollectionItem()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document1 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", new List<string>() { "foo1", "foo2" })
                        .Set("bar", 11111)
                        .Run();

                    ODocument document2 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", new List<string>() { "foo1", "foo2" })
                        .Set("bar", 12345)
                        .Run();

                    int documentsUpdated = database
                        .Update(document2)
                        .Remove("foo", "foo2")
                        .Run();

                    Assert.Equal(documentsUpdated, 1);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .Where("bar").Equals(12345)
                        .ToList();

                    Assert.Equal(documents.Count, 1);

                    Assert.Equal(documents[0].ORID, document2.ORID);
                    Assert.Equal(documents[0].OClassName, document2.OClassName);

                    List<string> foos = new List<string>() { "foo1" };

                    Assert.Equal(documents[0].GetField<List<string>>("foo").Count, foos.Count);
                    Assert.Equal(documents[0].GetField<List<string>>("foo")[0], foos[0]);

                    Assert.Equal(documents[0].GetField<int>("bar"), document2.GetField<int>("bar"));
                }
            }
        }

        [Fact]
        public void ShouldUpdateRemoveFieldQuery()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class("TestClass")
                        .Run();

                    ODocument document1 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", "foo string value1")
                        .Set("bar", 11111)
                        .Run();

                    ODocument document2 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", "foo string value2")
                        .Set("bar", 12345)
                        .Run();

                    int documentsUpdated = database
                        .Update(document2)
                        .Remove("bar")
                        .Run();

                    Assert.Equal(documentsUpdated, 1);

                    List<ODocument> documents = database
                        .Select()
                        .From("TestClass")
                        .Where("foo").Equals("foo string value2")
                        .ToList();

                    Assert.Equal(documents.Count, 1);

                    Assert.Equal(documents[0].ORID, document2.ORID);
                    Assert.Equal(documents[0].OClassName, document2.OClassName);
                    Assert.Equal(documents[0].GetField<string>("foo"), document2.GetField<string>("foo"));
                    Assert.False(documents[0].HasField("bar"));
                }
            }
        }
    }
}
