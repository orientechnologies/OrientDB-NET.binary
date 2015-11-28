using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Issues
{
    
    public class GitHub_issue83 : IDisposable
    {
        TestDatabaseContext _context;
        ODatabase _database;
        
        public GitHub_issue83()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
        }

        public void Dispose()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Fact]
        public void ShouldInsertValuesWithNewLines()
        {
            _database
                         .Create.Class("TestClass")
                         .Run();

            ODocument document = new ODocument();
            document.OClassName = "TestClass";
            document
                .SetField("foo", "foo string value \r\n test te5435ttrtr")
                .SetField("bar", 12345);

            ODocument insertedDocument = _database
                .Insert(document)
                .Run();

            Assert.True(insertedDocument.ORID != null);
            Assert.Equal(insertedDocument.OClassName, "TestClass");
            Assert.Equal(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
            Assert.Equal(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));
        }
    }
}
