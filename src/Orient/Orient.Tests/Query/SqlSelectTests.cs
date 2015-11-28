using System;
using System.Collections.Generic;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlSelectTests
    {
        [Fact]
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

                    Assert.Equal(documents.Count, 1);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.Equal(documents[i].ORID, document2.ORID);
                        Assert.Equal(documents[i].OClassName, document2.OClassName);
                        Assert.Equal(documents[i].GetField<string>("foo"), document2.GetField<string>("foo"));
                        Assert.Equal(documents[i].GetField<int>("bar"), document2.GetField<int>("bar"));
                    }
                }
            }
        }

        [Fact]
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

                    Assert.Equal(documents.Count, 2);

                    for (int i = 0; i < documents.Count; i++)
                    {
                        Assert.True(documents[i].ORID != null);
                        Assert.Equal(documents[i].OClassName, document.OClassName);
                        Assert.True(documents[i].HasField("foo"));
                        Assert.True(documents[i].HasField("bar"));
                    }
                }
            }
        }

        [Fact]
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

                    Assert.Equal(documents.Count, 2);

                    Assert.Equal(documents[0].ORID, document1.ORID);
                    Assert.Equal(documents[0].OClassName, document1.OClassName);
                    Assert.Equal(documents[0].Name, document1.Name);
                    Assert.Equal(documents[0].Surname, document1.Surname);

                    Assert.Equal(documents[1].ORID, document2.ORID);
                    Assert.Equal(documents[1].OClassName, document2.OClassName);
                    Assert.Equal(documents[1].Name, document2.Name);
                    Assert.Equal(documents[1].Surname, document2.Surname);
                }
            }
        }
        
        [Fact]
        public void ShouldSelectAsAnd()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OVertex>()
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

                    Assert.Equal(result.Count, 2);
                    Assert.Equal(result[0].GetField<string>("CustomFoo"), obj1.Foo);
                    Assert.Equal(result[0].GetField<int>("CustomBar"), obj1.Bar);
                    Assert.Equal(result[1].GetField<string>("CustomFoo"), obj2.Foo);
                    Assert.Equal(result[1].GetField<int>("CustomBar"), obj2.Bar);
                }
            }
        }

        [Fact]
        public void ShouldSelectSkipLimit()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OVertex>()
                        .Run();

                    TestVertexClass obj1 = new TestVertexClass();
                    obj1.Foo = "foo string value1";
                    obj1.Bar = 1;

                    TestVertexClass obj2 = new TestVertexClass();
                    obj2.Foo = "foo string value2";
                    obj2.Bar = 2;

                    TestVertexClass obj3 = new TestVertexClass();
                    obj3.Foo = "foo string value3";
                    obj3.Bar = 3;

                    TestVertexClass obj4 = new TestVertexClass();
                    obj4.Foo = "foo string value4";
                    obj4.Bar = 4;

                    TestVertexClass obj5 = new TestVertexClass();
                    obj5.Foo = "foo string value5";
                    obj5.Bar = 5;

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj1)
                        .Run();

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj2)
                        .Run();

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj3)
                        .Run();

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj4)
                        .Run();

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj5)
                        .Run();

                    List<ODocument> result = database
                        .Select("Foo").As("CustomFoo")
                        .Also("Bar").As("CustomBar")
                        .From<TestVertexClass>()
                        .Skip(2)
                        .Limit(2)
                        .ToList();

                    Assert.Equal(result.Count, 2);
                    Assert.Equal(result[0].GetField<string>("CustomFoo"), obj3.Foo);
                    Assert.Equal(result[0].GetField<int>("CustomBar"), obj3.Bar);
                    Assert.Equal(result[1].GetField<string>("CustomFoo"), obj4.Foo);
                    Assert.Equal(result[1].GetField<int>("CustomBar"), obj4.Bar);
                }
            }
        }

        [Fact]
        public void ShouldSelectOrderByDescending()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // prerequisites
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OVertex>()
                        .Run();

                    TestVertexClass obj1 = new TestVertexClass();
                    obj1.Foo = "foo string value1";
                    obj1.Bar = 1;

                    TestVertexClass obj2 = new TestVertexClass();
                    obj2.Foo = "foo string value2";
                    obj2.Bar = 2;

                    TestVertexClass obj3 = new TestVertexClass();
                    obj3.Foo = "foo string value3";
                    obj3.Bar = 3;

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj1)
                        .Run();

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj2)
                        .Run();

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj3)
                        .Run();

                    List<ODocument> result = database
                        .Select("Foo").As("CustomFoo")
                        .Also("Bar").As("CustomBar")
                        .From<TestVertexClass>()
                        .OrderBy("CustomBar").Descending()
                        .ToList();

                    Assert.Equal(result.Count, 3);
                    Assert.Equal(result[0].GetField<string>("CustomFoo"), obj3.Foo);
                    Assert.Equal(result[0].GetField<int>("CustomBar"), obj3.Bar);
                    Assert.Equal(result[1].GetField<string>("CustomFoo"), obj2.Foo);
                    Assert.Equal(result[1].GetField<int>("CustomBar"), obj2.Bar);
                    Assert.Equal(result[2].GetField<string>("CustomFoo"), obj1.Foo);
                    Assert.Equal(result[2].GetField<int>("CustomBar"), obj1.Bar);
                }
            }
        }
    }
}
