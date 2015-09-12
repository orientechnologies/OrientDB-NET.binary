using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture]
    public class GitHub_issue84
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
        public void ShouldInsertValuesWithBackslash()
        {
            _database
                         .Create.Class("TestClass")
                         .Run();

            ODocument document = new ODocument();
            document.OClassName = "TestClass";
            document
                .SetField("foo", "foo string value \\ test te5435ttrtr")
                .SetField("bar", 12345);

            ODocument insertedDocument = _database
                .Insert(document)
                .Run();

            Assert.IsTrue(insertedDocument.ORID != null);
            Assert.AreEqual(insertedDocument.OClassName, "TestClass");
            Assert.AreEqual(insertedDocument.GetField<string>("foo"), document.GetField<string>("foo"));
            Assert.AreEqual(insertedDocument.GetField<int>("bar"), document.GetField<int>("bar"));
        }
    }
}
