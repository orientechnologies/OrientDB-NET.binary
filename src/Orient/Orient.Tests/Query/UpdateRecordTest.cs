using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class UpdateRecordTest
    {
        [TestMethod]
        public void ShouldUpdateRecord()
        {
            using (var testContext = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                // prerequisites
                database
                    .Create.Class("TestClass")
                    .Run();

                ODocument document = new ODocument { OClassName = "TestClass" }
                    .SetField("foo", "foo string value")
                    .SetField("bar", 12345);

                var cd = database.Create.Document(document).Run();
                cd.SetField("bar", 54321);

                var ud = database.Update(cd).Run();
                cd.SetField("bar", 9876);
                var ud1 = database.Update(cd).Run();
            }
        }
    }
}
