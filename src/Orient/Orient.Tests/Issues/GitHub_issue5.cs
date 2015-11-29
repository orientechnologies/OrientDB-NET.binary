using System;
using System.Globalization;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Issues
{
    
    public class GitHub_issue5 : IDisposable
    {
        TestDatabaseContext _context;
        ODatabase _database;
        
        public GitHub_issue5()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            _database.Create.Class("TestVertex").Extends<OVertex>().Run();
        }

        public void Dispose()
        {
            _database.Dispose();
            _context.Dispose();
        }


        [Fact]
        public void TestGermanFloatCulture()
        {
            var floatValue = "108,4";

            float @float;
            float.TryParse(floatValue, NumberStyles.Any, CultureInfo.GetCultureInfo("de-DE"), out @float);

            var doc = new ODocument { OClassName = "TestVertex" }
                .SetField("floatField", @float);

            var insertedDoc = _database.Insert(doc).Run();

            Assert.NotNull(insertedDoc.ORID);
            Assert.Equal(doc.GetField<float>("@float"), insertedDoc.GetField<float>("@float"));
        }
    }
}
