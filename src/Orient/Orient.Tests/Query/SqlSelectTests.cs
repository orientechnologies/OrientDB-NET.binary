using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Sql
{
    [TestClass]
    public class SqlSelectTests
    {
        [TestMethod]
        public void ShouldSelectUntyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    string className = "TestVertexClass";

                    ODataObject fields1 = new ODataObject();
                    fields1.Set<string>("foo", "foo string value1");
                    fields1.Set<int>("bar", 12345);

                    ODataObject fields2 = new ODataObject();
                    fields2.Set<string>("foo", "foo string value2");
                    fields2.Set<int>("bar", 54321);

                    // create test class
                    database
                        .Create.Class("TestVertexClass")
                        .Extends("OGraphVertex")
                        .Run();

                    // load database with some testing data
                    database
                        .Create.Vertex(className)
                        .Set(fields1)
                        .Run();

                    database
                        .Create.Vertex(className)
                        .Set(fields2)
                        .Run();

                    // perform simple select
                    List<ORecord> result = database
                        .Select("foo", "bar")
                        .From("TestVertexClass")
                        .Run();

                    Assert.AreEqual(result.Count, 2);
                    Assert.AreEqual(result[0].GetField<string>("foo"), fields1.Get<string>("foo"));
                    Assert.AreEqual(result[0].GetField<int>("bar"), fields1.Get<int>("bar"));
                    Assert.AreEqual(result[1].GetField<string>("foo"), fields2.Get<string>("foo"));
                    Assert.AreEqual(result[1].GetField<int>("bar"), fields2.Get<int>("bar"));
                }
            }
        }

        [TestMethod]
        public void ShouldSelectTyped()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    TestVertexClass obj1 = new TestVertexClass();
                    obj1.Foo = "foo string value1";
                    obj1.Bar = 12345;

                    TestVertexClass obj2 = new TestVertexClass();
                    obj2.Foo = "foo string value2";
                    obj2.Bar = 54321;

                    // create test class
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends("OGraphVertex")
                        .Run();

                    // load database with some testing data
                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj1)
                        .Run();

                    database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj2)
                        .Run();

                    // perform simple select
                    List<ORecord> result = database
                        .Select("Foo", "Bar")
                        .From<TestVertexClass>()
                        .Run();

                    Assert.AreEqual(result.Count, 2);
                    Assert.AreEqual(result[0].GetField<string>("Foo"), obj1.Foo);
                    Assert.AreEqual(result[0].GetField<int>("Bar"), obj1.Bar);
                    Assert.AreEqual(result[1].GetField<string>("Foo"), obj2.Foo);
                    Assert.AreEqual(result[1].GetField<int>("Bar"), obj2.Bar);
                }
            }
        }
    }
}
