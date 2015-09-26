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
        public void ShouldInsertObjectWithNestedObject()
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
            profile.NestedObject = new TestNestedObject() { Strings = new List<string>() { "Address 1", "address 2" }, Test1 = "test", Test2 = null, Dictionary = new Dictionary<string, string>() { { "Key1", "Value1" }, { "Key2", "Value2" } }, ObjectHashSet = new HashSet<TestNestedObject2>() { new TestNestedObject2() { Test1 = "Test1", Test2 = 2 } } };

            TestProfileClassExtended profile2 = new TestProfileClassExtended();
            profile2.Name = "Richard";
            profile2.Surname = "Benson";
            profile2.NestedObject = new TestNestedObject() { Strings = new List<string>() { "Address 3", "address 4" }, Test1 = "test2", Test2 = 3, Dictionary = new Dictionary<string, string>() { { "Key3", "Value3" }, { "Key4", "Value4" } }, ObjectHashSet = new HashSet<TestNestedObject2>() { new TestNestedObject2() { Test1 = "Test1", Test2 = null } } };
            profile2.Dictionary = new Dictionary<string, string>();
            profile2.Dictionary.Add("Key3", "Value3");
            profile2.Dictionary.Add("Key4", "Value4");

            TestProfileClassExtended profile3 = new TestProfileClassExtended();
            profile3.NestedObject = new TestNestedObject();

            TestProfileClassExtended insertedDocument = _database
                .Insert(profile)
                .Run<TestProfileClassExtended>();

            TestProfileClassExtended insertedDocument2 = _database
                .Insert(profile2)
                .Run<TestProfileClassExtended>();

            TestProfileClassExtended insertedDocument3 = _database
                .Insert(profile3)
                .Run<TestProfileClassExtended>();

            List<TestProfileClassExtended> documents = _database
                .Select()
                .From<TestProfileClassExtended>()
                .ToList<TestProfileClassExtended>();

            Assert.Greater(documents.Count, 0);
            Assert.IsNotNull(documents[0].NestedObject);
            Assert.IsNotNull(documents[0].NestedObject.Strings);
            Assert.Greater(documents[0].NestedObject.Strings.Count, 0);
            Assert.IsNotNull(documents[0].NestedObject.Test1);
            Assert.IsNull(documents[0].NestedObject.Test2);
            Assert.Greater(documents[0].NestedObject.Dictionary.Keys.Count, 0);
            Assert.Greater(documents[0].Dictionary.Keys.Count, 0);
            Assert.Greater(documents[0].NestedObject.ObjectHashSet.Count, 0);
            Assert.IsNotNull(documents[0].NestedObject.ObjectHashSet.First().Test1);
            Assert.IsNotNull(documents[0].NestedObject.ObjectHashSet.First().Test2);

            Assert.IsNotNull(documents[1].NestedObject);
            Assert.IsNotNull(documents[1].NestedObject.Strings);
            Assert.Greater(documents[1].NestedObject.Strings.Count, 0);
            Assert.IsNotNull(documents[1].NestedObject.Test1);
            Assert.IsNotNull(documents[1].NestedObject.Test2);
            Assert.Greater(documents[1].NestedObject.Dictionary.Keys.Count, 0);
            Assert.Greater(documents[1].Dictionary.Keys.Count, 0);
            Assert.IsNotNull(documents[1].NestedObject.ObjectHashSet.First().Test1);
            Assert.IsNull(documents[1].NestedObject.ObjectHashSet.First().Test2);

            Assert.IsNotNull(documents[2].NestedObject);
        }

        public class TestProfileClassExtended : TestProfileClass
        {
            public TestNestedObject NestedObject { get; set; }

            public Dictionary<string, string> Dictionary { get; set; }
        }

        public class TestNestedObject
        {
            public List<string> Strings { get; set; }

            public HashSet<TestNestedObject2> ObjectHashSet { get; set; }

            public Dictionary<string, string> Dictionary { get; set; }

            public string Test1 { get; set; }

            public int? Test2 { get; set; }
        }

        public class TestNestedObject2
        {
            public string Test1 { get; set; }

            public int? Test2 { get; set; }
        }
    }
}
