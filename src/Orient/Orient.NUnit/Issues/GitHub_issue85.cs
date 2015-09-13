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
    public class GitHub_issue85
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

            Assert.IsTrue(insertedDocument.ORID != null);
            Assert.AreEqual(insertedDocument.OClassName, typeof(TestProfileClassExtended).Name);
            Assert.AreEqual(insertedDocument.Name, profile.Name);
            Assert.AreEqual(insertedDocument.Surname, profile.Surname);
            Assert.IsNotNull(insertedDocument.StringArray);
            Assert.AreEqual(insertedDocument.StringArray.Length, 2);
        }

        [Test]
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

            Assert.IsTrue(insertedDocument.ORID != null);
            Assert.AreEqual(insertedDocument.OClassName, typeof(TestProfileClassExtended).Name);
            Assert.AreEqual(insertedDocument.Name, profile.Name);
            Assert.AreEqual(insertedDocument.Surname, profile.Surname);
            Assert.IsNotNull(insertedDocument.StringArray);
            Assert.AreEqual(insertedDocument.StringArray.Length, 2);
            Assert.IsNotNull(insertedDocument.StringArray[0]);
            Assert.IsNull(insertedDocument.StringArray[1]);
        }

        public class TestProfileClassExtended : TestProfileClass
        {
            public string[] StringArray { get; set; }
        }
    }
}
