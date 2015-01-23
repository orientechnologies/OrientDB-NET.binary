using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture]
    public class GitHub_issue4
    {
        TestDatabaseContext _context;
        ODatabase _database;

        [SetUp]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            _database.Create.Class("TestVertex").Run();
            _database.Create.Property("_datetime", OType.DateTime).Class("TestVertex").Run();
        }

        [TearDown]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Test]
        [Ignore]
        public void ShouldAxceptAllRangeOfDateTime()
        {
            var dtNow = DateTime.Now;
            var dtUTCNow = DateTime.UtcNow;

            // Gregorian calendar has hole between 1582-10-04 and 1582-10-15
            // need adjust values

            var dt15821015 = new DateTime(1582, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc);
            var dt15821014 = new DateTime(1582, 10, 14, 10, 0, 0, 0, DateTimeKind.Utc);
            var dt15821004 = new DateTime(1582, 10, 4, 0, 0, 0, 0, DateTimeKind.Utc);
            var dt15821003 = new DateTime(1582, 10, 3, 0, 0, 0, 0, DateTimeKind.Utc);            
            var dt01000229 = new DateTime(0100, 2, 28, 0, 0, 0, 0, DateTimeKind.Utc);            
            
            var unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

            var doc0 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", dt15821014);
            
            //var doc0 = new ODocument { OClassName = "TestVertex" }
            //    .SetField<DateTime>("_datetime", dt15821015);

            var doc1 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", dt15821004);

            var doc2 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", dt15821003);

            var doc3 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", dt01000229);

            var doc4 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", default(DateTime));

            var doc5 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            var doc6 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", dtNow);

            var doc7 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", dtUTCNow);

            var doc8 = new ODocument { OClassName = "TestVertex" }
                .SetField<DateTime>("_datetime", DateTime.MaxValue);

            var insertedDoc0 = _database
                .Insert(doc0)
                .Run();

            var insertedDoc1 = _database
                .Insert(doc1)
                .Run();

            var insertedDoc2 = _database
                .Insert(doc2)
                .Run();

            var insertedDoc3 = _database
                .Insert(doc3)
                .Run();

            var insertedDoc4 = _database
                .Insert(doc4)
                .Run();

            var insertedDoc5 = _database
                .Insert(doc5)
                .Run();

            var insertedDoc6 = _database
                .Insert(doc6)
                .Run();

            var insertedDoc7 = _database
                .Insert(doc7)
                .Run();

            var insertedDoc8 = _database
                .Insert(doc8)
                .Run();

        }
    }
}
