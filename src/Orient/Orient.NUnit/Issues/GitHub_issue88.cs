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
    public class GitHub_issue88
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
        public void ShouldInsertObjectWithNestedObjectList()
        {
            // prerequisites
            _database
                .Create.Class<TestProfileClassExtended>()
                .Run();

            TestProfileClassExtended profile = new TestProfileClassExtended();
            profile.Name = "Johny";
            profile.Surname = "Bravo";
            profile.Dictionary = new Dictionary<string, string>();
            profile.Dictionary.Add("Key1", "Value1");
            profile.Dictionary.Add("Key2", "Value2");
            profile.NestedObject = new TestNestedObject() { Strings = new List<string>() { "Address 1", "address 2" }, Dictionary = new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } } };

            TestProfileClassExtended profile2 = new TestProfileClassExtended();
            profile2.Name = "Richard";
            profile2.Surname = "Benson";
            profile2.NestedObject = new TestNestedObject() { Strings = new List<string>() { "Address 3", "address 4" }, Dictionary = new Dictionary<string, string>() { { "Key3", "Value3" }, { "Key4", "Value4" } } };
            profile2.Dictionary = new Dictionary<string, string>();
            profile2.Dictionary.Add("Key3", "Value3");
            profile2.Dictionary.Add("Key4", "Value4");

            TestProfileClassExtended insertedDocument = _database
                .Insert(profile)
                .Run<TestProfileClassExtended>();

            TestProfileClassExtended insertedDocument2 = _database
                .Insert(profile2)
                .Run<TestProfileClassExtended>();

            List<TestProfileClassExtended> documents = _database
                .Select()
                .From<TestProfileClassExtended>()
                .ToList<TestProfileClassExtended>();

            Assert.Greater(documents.Count, 0);
            Assert.IsNotNull(documents[0].NestedObject);
            Assert.IsNotNull(documents[0].NestedObject.Strings);
            Assert.Greater(documents[0].NestedObject.Strings.Count, 0);
            Assert.Greater(documents[0].NestedObject.Dictionary.Keys.Count, 0);
            Assert.Greater(documents[0].Dictionary.Keys.Count, 0);
        }

        public class TestProfileClassExtended : TestProfileClass
        {
            public TestNestedObject NestedObject { get; set; }

            public Dictionary<string, string> Dictionary { get; set; }
        }

        public class TestNestedObject
        {
            public List<string> Strings { get; set; }

            public Dictionary<string, string> Dictionary { get; set; }
        }
    }
}
