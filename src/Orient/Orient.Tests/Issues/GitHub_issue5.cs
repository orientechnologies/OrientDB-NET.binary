using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestClass]
    public class GitHub_issue5
    {
        TestDatabaseContext _context;
        ODatabase _database;

        [TestInitialize]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            _database.Create.Class("TestVertex").Extends<OVertex>().Run();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }


        [TestMethod]
        public void TestGermanFloatCulture()
        {
            var floatValue = "108,4";

            float @float;
            float.TryParse(floatValue, NumberStyles.Any, CultureInfo.GetCultureInfo("de-DE"), out @float);

            var doc = new ODocument { OClassName = "TestVertex" }
                .SetField("floatField", @float);

            var insertedDoc = _database.Insert(doc).Run();

            Assert.IsNotNull(insertedDoc.ORID);
            Assert.AreEqual(doc.GetField<float>("@float"), insertedDoc.GetField<float>("@float"));
        }
    }
}
