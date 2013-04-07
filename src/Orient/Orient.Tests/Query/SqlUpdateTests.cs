using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlUpdateTests
    {
        [TestMethod]
        public void ShouldUpdateSet()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OGraphVertex>()
                        .Run();

                    TestVertexClass obj1 = new TestVertexClass();
                    obj1.Foo = "foo string value 1";
                    obj1.Bar = 12345;

                    ORecord createdRecord1 = database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj1)
                        .Run();

                    Assert.AreEqual(createdRecord1.GetField<string>("Foo"), obj1.Foo);
                    Assert.AreEqual(createdRecord1.GetField<int>("Bar"), obj1.Bar);

                    TestVertexClass obj2 = new TestVertexClass();
                    obj2.Foo = "foo string value 2";
                    obj2.Bar = 123456;

                    ORecord createdRecord2 = database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj2)
                        .Run();

                    Assert.AreEqual(createdRecord2.GetField<string>("Foo"), obj2.Foo);
                    Assert.AreEqual(createdRecord2.GetField<int>("Bar"), obj2.Bar);

                    int recordsUpdated = database
                        .Update.Class<TestVertexClass>()
                        .Set("Baz", "baz string value")
                        .Run();

                    Assert.AreEqual(recordsUpdated, 2);

                    List<ORecord> updatedRecords = database
                        .Select()
                        .From<TestVertexClass>()
                        .ToList();

                    Assert.AreEqual(updatedRecords.Count, 2);
                    Assert.AreEqual(updatedRecords[0].GetField<string>("Foo"), obj1.Foo);
                    Assert.AreEqual(updatedRecords[0].GetField<int>("Bar"), obj1.Bar);
                    Assert.AreEqual(updatedRecords[0].GetField<string>("Baz"), "baz string value");
                    Assert.AreEqual(updatedRecords[1].GetField<string>("Foo"), obj2.Foo);
                    Assert.AreEqual(updatedRecords[1].GetField<int>("Bar"), obj2.Bar);
                    Assert.AreEqual(updatedRecords[1].GetField<string>("Baz"), "baz string value");
                }
            }
        }

        [TestMethod]
        public void ShouldUpdateSetWhere()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OGraphVertex>()
                        .Run();

                    TestVertexClass obj1 = new TestVertexClass();
                    obj1.Foo = "foo string value 1";
                    obj1.Bar = 12345;

                    ORecord createdRecord1 = database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj1)
                        .Run();

                    Assert.AreEqual(createdRecord1.GetField<string>("Foo"), obj1.Foo);
                    Assert.AreEqual(createdRecord1.GetField<int>("Bar"), obj1.Bar);

                    TestVertexClass obj2 = new TestVertexClass();
                    obj2.Foo = "foo string value 2";
                    obj2.Bar = 123456;

                    ORecord createdRecord2 = database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj2)
                        .Run();

                    Assert.AreEqual(createdRecord2.GetField<string>("Foo"), obj2.Foo);
                    Assert.AreEqual(createdRecord2.GetField<int>("Bar"), obj2.Bar);

                    int recordsUpdated = database
                        .Update.Class<TestVertexClass>()
                        .Set("Foo", "new string value")
                        .Set("Bar", 54321)
                        .Set("Baz", "baz string value")
                        .Where("@rid").Equals(createdRecord1.ORID)
                        .Run();

                    Assert.AreEqual(recordsUpdated, 1);

                    List<ORecord> updatedRecords = database
                        .Select()
                        .From<TestVertexClass>()
                        .ToList();

                    Assert.AreEqual(updatedRecords.Count, 2);
                    Assert.AreEqual(updatedRecords[0].GetField<string>("Foo"), "new string value");
                    Assert.AreEqual(updatedRecords[0].GetField<int>("Bar"), 54321);
                    Assert.AreEqual(updatedRecords[0].GetField<string>("Baz"), "baz string value");
                    Assert.AreEqual(updatedRecords[1].GetField<string>("Foo"), obj2.Foo);
                    Assert.AreEqual(updatedRecords[1].GetField<int>("Bar"), obj2.Bar);
                }
            }
        }

        [TestMethod]
        public void ShouldUpdateRecordSet()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OGraphVertex>()
                        .Run();

                    TestVertexClass obj1 = new TestVertexClass();
                    obj1.Foo = "foo string value 1";
                    obj1.Bar = 12345;

                    ORecord createdRecord1 = database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj1)
                        .Run();

                    Assert.AreEqual(createdRecord1.GetField<string>("Foo"), obj1.Foo);
                    Assert.AreEqual(createdRecord1.GetField<int>("Bar"), obj1.Bar);

                    TestVertexClass obj2 = new TestVertexClass();
                    obj2.Foo = "foo string value 2";
                    obj2.Bar = 123456;

                    ORecord createdRecord2 = database
                        .Create.Vertex<TestVertexClass>()
                        .Set(obj2)
                        .Run();

                    Assert.AreEqual(createdRecord2.GetField<string>("Foo"), obj2.Foo);
                    Assert.AreEqual(createdRecord2.GetField<int>("Bar"), obj2.Bar);

                    int recordsUpdated = database
                        .Update.Record(createdRecord1.ORID)
                        .Set("Foo", "new string value")
                        .Set("Bar", 54321)
                        .Set("Baz", "baz string value")
                        .Run();

                    Assert.AreEqual(recordsUpdated, 1);

                    List<ORecord> updatedRecords = database
                        .Select()
                        .From<TestVertexClass>()
                        .ToList();

                    Assert.AreEqual(updatedRecords.Count, 2);
                    Assert.AreEqual(updatedRecords[0].GetField<string>("Foo"), "new string value");
                    Assert.AreEqual(updatedRecords[0].GetField<int>("Bar"), 54321);
                    Assert.AreEqual(updatedRecords[0].GetField<string>("Baz"), "baz string value");
                    Assert.AreEqual(updatedRecords[1].GetField<string>("Foo"), obj2.Foo);
                    Assert.AreEqual(updatedRecords[1].GetField<int>("Bar"), obj2.Bar);
                }
            }
        }

        [TestMethod]
        public void ShouldUpdateAdd()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class<TestVertexClass>()
                        .Extends<OGraphVertex>()
                        .Run();

                    ODocument document = new ODocument();
                    document.SetField<List<string>>("FooCollection", new List<string> { "foo 1", "foo 2" });

                    ORecord createdRecord1 = database
                        .Create.Vertex<TestVertexClass>()
                        .Set(document)
                        .Run();

                    List<string> fooCollection = createdRecord1.GetField<List<string>>("FooCollection");
                    
                    Assert.AreEqual(fooCollection.Count, 2);

                    int recordsUpdated = database
                        .Update.Record(createdRecord1.ORID)
                        .Add("FooCollection", "foo 3")
                        .Run();

                    Assert.AreEqual(recordsUpdated, 1);

                    List<ORecord> updatedRecords = database
                        .Select()
                        .From<TestVertexClass>()
                        .ToList();

                    Assert.AreEqual(updatedRecords.Count, 1);

                    fooCollection = updatedRecords[0].GetField<List<string>>("FooCollection");

                    Assert.AreEqual(fooCollection.Count, 3);
                    Assert.AreEqual(fooCollection[0], "foo 1");
                    Assert.AreEqual(fooCollection[1], "foo 2");
                    Assert.AreEqual(fooCollection[2], "foo 3");
                }
            }
        }


    }
}
