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
        public void TestMethod1()
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

                    database.Create.Class(className).Extends("OGraphVertex").Run();

                    //database.Create.Vertex(className, fields1);
                    //database.Create.Vertex(className, fields2);

                    /*List<ORecord> result = database.SQL
                        .Select("foo")
                        .From(className)
                        .ToList();*/
                    List<ORecord> result = database.SQL
                        .Select("foo")
                        .From<TestVertexClass>()
                        .ToList();
                }
            }
        }
    }
}
