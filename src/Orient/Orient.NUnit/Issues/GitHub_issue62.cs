using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture(Category = "issue")]
    public class GitHub_issue62
    {
        TestDatabaseContext _context;
        ODatabase _database;

        [SetUp]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);

            _database.Create.Class<Person>().Run();
        }

        [TearDown]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Test]
        public void ShouldInsertRetrieveSingleDictionaryProperty()
        {
            var person = new Person
            {
                Name = "Luca",
                EmbededMap = new Dictionary<string, int>()
            };
            person.EmbededMap.Add("test", 100);

            var document = _database.Insert<Person>(person).Run();
            var createdPerson = document.To<Person>();
            Assert.That(createdPerson.EmbededMap["test"], Is.EqualTo(100));
        }

        public class Person
        {
            public string Name { get; set; }
            public Dictionary<string, int> EmbededMap { get; set; }
        }
    }
}
