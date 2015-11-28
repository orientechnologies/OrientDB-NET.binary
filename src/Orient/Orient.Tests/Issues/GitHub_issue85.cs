using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Issues
{
    
    public class GitHub_issue85 : IDisposable
    {
        TestDatabaseContext _context;
        ODatabase _database;
        
        public GitHub_issue85()
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
        public void ShouldInsertObjectWithStringArray()
        {
            // prerequisites
            _database
                .Create.Class<TestProfileClassExtended>()
                .Run();

            TestProfileClassExtended profile = new TestProfileClassExtended();
            profile.Name = "Johny";
            profile.Surname = "Bravo";
            profile.StringArray = new string[] { "Test string", "Test string 2" };

            var insert = _database
                .Insert(profile);

            TestProfileClassExtended insertedDocument = insert
                .Run<TestProfileClassExtended>();

            Assert.True(insertedDocument.ORID != null);
            Assert.Equal(insertedDocument.OClassName, typeof(TestProfileClassExtended).Name);
            Assert.Equal(insertedDocument.Name, profile.Name);
            Assert.Equal(insertedDocument.Surname, profile.Surname);
            Assert.NotNull(insertedDocument.StringArray);
            Assert.Equal(insertedDocument.StringArray.Length, 2);
        }

        [Fact]
        public void ShouldInsertObjectWithNullStringArrayValues()
        {
            // prerequisites
            _database
                .Create.Class<TestProfileClassExtended>()
                .Run();

            TestProfileClassExtended profile = new TestProfileClassExtended();
            profile.Name = "Johny";
            profile.Surname = "Bravo";
            profile.StringArray = new string[] { "Test string", null };

            var insert = _database
                .Insert(profile);

            TestProfileClassExtended insertedDocument = insert
                .Run<TestProfileClassExtended>();

            Assert.True(insertedDocument.ORID != null);
            Assert.Equal(insertedDocument.OClassName, typeof(TestProfileClassExtended).Name);
            Assert.Equal(insertedDocument.Name, profile.Name);
            Assert.Equal(insertedDocument.Surname, profile.Surname);
            Assert.NotNull(insertedDocument.StringArray);
            Assert.Equal(insertedDocument.StringArray.Length, 2);
            Assert.NotNull(insertedDocument.StringArray[0]);
            Assert.Null(insertedDocument.StringArray[1]);
        }

        public class TestProfileClassExtended : TestProfileClass
        {
            public string[] StringArray { get; set; }
        }
    }
}
