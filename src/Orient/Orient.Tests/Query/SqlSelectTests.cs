using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlSelectTests
    {
        [TestMethod]
        public void ShouldSelectFromDocumentOrid()
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
                        .Set("bar", 12345)
                        .Run();

                    ODocument document2 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    List<ODocument> documents = database
                        .Select()
                        .From(document2)
                        .ToList();

                    Assert.AreEqual(documents.Count, 1);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.AreEqual(documents[i].ORID, document2.ORID);
                        Assert.AreEqual(documents[i].OClassName, document2.OClassName);
                        Assert.AreEqual(documents[i].GetField<string>("foo"), document2.GetField<string>("foo"));
                        Assert.AreEqual(documents[i].GetField<int>("bar"), document2.GetField<int>("bar"));
                    }
                }
            }
        }

        [TestMethod]
        public void ShouldSelectFromDocumentOClassNameQuery()
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
                        .Set("bar", 12345)
                        .Run();

                    ODocument document2 = database
                        .Insert()
                        .Into("TestClass")
                        .Set("foo", "foo string value2")
                        .Set("bar", 54321)
                        .Run();

                    ODocument document = new ODocument();
                    document.OClassName = "TestClass";

                    List<ODocument> documents = database
                        .Select()
                        .From(document)
                        .ToList();

                    Assert.AreEqual(documents.Count, 2);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.IsTrue(documents[i].ORID != null);
                        Assert.AreEqual(documents[i].OClassName, document.OClassName);
                        Assert.IsTrue(documents[i].HasField("foo"));
                        Assert.IsTrue(documents[i].HasField("bar"));
                    }
                }
            }
        }

        [TestMethod]
        public void ShouldSelectToObjectList()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestProfileClass>()
                        .Run();

                    TestProfileClass document1 = database
                        .Insert()
                        .Into<TestProfileClass>()
                        .Set("Name", "Johny")
                        .Set("Surname", "Bravo")
                        .Run<TestProfileClass>();

                    TestProfileClass document2 = database
                        .Insert()
                        .Into<TestProfileClass>()
                        .Set("Name", "Johny")
                        .Set("Surname", "Bravo")
                        .Run<TestProfileClass>();

                    List<TestProfileClass> documents = database
                        .Select()
                        .From<TestProfileClass>()
                        .ToList<TestProfileClass>();

                    Assert.AreEqual(documents.Count, 2);

                    Assert.AreEqual(documents[0].ORID, document1.ORID);
                    Assert.AreEqual(documents[0].OClassName, document1.OClassName);
                    Assert.AreEqual(documents[0].Name, document1.Name);
                    Assert.AreEqual(documents[0].Surname, document1.Surname);

                    Assert.AreEqual(documents[1].ORID, document2.ORID);
                    Assert.AreEqual(documents[1].OClassName, document2.OClassName);
                    Assert.AreEqual(documents[1].Name, document2.Name);
                    Assert.AreEqual(documents[1].Surname, document2.Surname);
                }
            }
        }
        
        [TestMethod]
        public void ShouldSelectAsAnd()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OGraphVertex>()
                        .Run();

                    TestVertexClass obj1 = new TestVertexClass();
                    obj1.Foo = "foo string value1";
                    obj1.Bar = 12345;

                    TestVertexClass obj2 = new TestVertexClass();
                    obj2.Foo = "foo string value2";
                    obj2.Bar = 54321;

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj1)
                        .Run();

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj2)
                        .Run();

                    List<ODocument> result = database
                        .Select("Foo").As("CustomFoo")
                        .Also("Bar").As("CustomBar")
                        .From<TestVertexClass>()
                        .ToList();

                    Assert.AreEqual(result.Count, 2);
                    Assert.AreEqual(result[0].GetField<string>("CustomFoo"), obj1.Foo);
                    Assert.AreEqual(result[0].GetField<int>("CustomBar"), obj1.Bar);
                    Assert.AreEqual(result[1].GetField<string>("CustomFoo"), obj2.Foo);
                    Assert.AreEqual(result[1].GetField<int>("CustomBar"), obj2.Bar);
                }
            }
        }
    }
}
