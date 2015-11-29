using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class LoadRecordTests
    {

        [Fact]
        public void TestLoadNoFetchPlan()
        {
            using (var testContext = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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
                var loaded = database.Load.ORID(insertedDocument.ORID).Run();
                Assert.Equal("TestClass", loaded.OClassName);
                Assert.Equal(document.GetField<string>("foo"), loaded.GetField<string>("foo"));
                Assert.Equal(document.GetField<int>("bar"), loaded.GetField<int>("bar"));
                Assert.Equal(insertedDocument.ORID, loaded.ORID);

            }
        }
        [Fact]
        public void TestLoadRawDataRecordType()
        {

            using (var testContext = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var config = database.Load.ORID(new ORID(0, 0)).Run();
                Assert.IsAssignableFrom<ODocument>(config);
                Assert.True(config.GetField<byte[]>("RawBytes").Length > 0);
            }

        }
        [Fact]
        public void TestLoadWithFetchPlanNoLinks()
        {
            using (var testContext = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
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
                var loaded = database.Load.ORID(insertedDocument.ORID).FetchPlan("*:1").Run();
                Assert.Equal("TestClass", loaded.OClassName);
                Assert.Equal(document.GetField<string>("foo"), loaded.GetField<string>("foo"));
                Assert.Equal(document.GetField<int>("bar"), loaded.GetField<int>("bar"));
                Assert.Equal(insertedDocument.ORID, loaded.ORID);

            }
        }

        [Fact]
        public void TestLoadWithFetchPlanWithLinks()
        {
            using (var testContext = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                // prerequisites
                database
                    .Create.Class("TestClass")
                    .Extends("V")
                    .Run();

                ODocument document = new ODocument()
                    .SetField("foo", "foo string value")
                    .SetField("bar", 12345);

                ODocument insertedDocument = database
                    .Insert(document)
                    .Into("TestClass")
                    .Run();

                for (int i = 0; i < 2; i++)
                {

                    ODocument document2 = new ODocument()
                        .SetField("foo", "bar string value")
                        .SetField("bar", 23456);

                    ODocument insertedDocument2 = database
                        .Insert(document2)
                        .Into("TestClass")
                        .Run();
                    database.Create.Edge("E").From(insertedDocument).To(insertedDocument2).Run();
                }


                var loaded = database.Load.ORID(insertedDocument.ORID).FetchPlan("*:1").Run();
                Assert.Equal("TestClass", loaded.OClassName);
                Assert.Equal(document.GetField<string>("foo"), loaded.GetField<string>("foo"));
                Assert.Equal(document.GetField<int>("bar"), loaded.GetField<int>("bar"));
                Assert.Equal(insertedDocument.ORID, loaded.ORID);

            }
        }
    }
}
