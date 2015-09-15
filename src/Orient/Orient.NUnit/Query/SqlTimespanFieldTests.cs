using NUnit.Framework;
using Orient.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class SqlTimespanFieldTests
    {
        TestDatabaseContext _context;
        ODatabase _database;

        [SetUp]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
        }

        [TearDown]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Test]
        public void ShouldInsertTimespanValues()
        {
            // prerequisites
            _database
                .Create.Class<TestTimeSpanClass>()
                .CreateProperties()
                .Run();

            TestTimeSpanClass testClass = new TestTimeSpanClass();

            testClass.TimespanProp = TimeSpan.FromDays(1);
            testClass.EmbeddedObj = new TestTimeSpanEmbeddedClass();
            testClass.TimespanProp = TimeSpan.FromHours(2);

            TestTimeSpanClass insertedDocument = _database
                .Insert(testClass)
                .Run<TestTimeSpanClass>();

        }

        public class TestTimeSpanClass
        {
            public TimeSpan TimespanProp { get; set; }

            public TimeSpan? NullableTimespanProp { get; set; }

            public TestTimeSpanEmbeddedClass EmbeddedObj { get; set; }
        }

        public class TestTimeSpanEmbeddedClass
        {
            public TimeSpan TimespanProp { get; set; }

            public TimeSpan? NullableTimespanProp { get; set; }
        }
    }
}
