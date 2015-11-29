using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Issues
{
    
    public class GitHub_issue86 : IDisposable
    {
        TestDatabaseContext _context;
        ODatabase _database;
        
        public GitHub_issue86()
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
        public void ShouldInsertObjectWithSingleNullArrayItem()
        {
            // prerequisites
            _database
                .Create.Class<TestProfileClassExtended>()
                .Run();

            TestProfileClassExtended profile = new TestProfileClassExtended();
            profile.Name = "Johny";
            profile.Surname = "Bravo";
            profile.StringArray = new string[] { null };

            var insert = _database
                .Insert(profile);

            TestProfileClassExtended insertedDocument = insert
                .Run<TestProfileClassExtended>();

            Assert.True(insertedDocument.ORID != null);
            Assert.Equal(insertedDocument.OClassName, typeof(TestProfileClassExtended).Name);
            Assert.Equal(insertedDocument.Name, profile.Name);
            Assert.Equal(insertedDocument.Surname, profile.Surname);
            Assert.NotNull(insertedDocument.StringArray);
            Assert.Equal(insertedDocument.StringArray.Length, 1);
        }

        public class TestProfileClassExtended : TestProfileClass
        {
            public string[] StringArray { get; set; }
        }
    }
}
