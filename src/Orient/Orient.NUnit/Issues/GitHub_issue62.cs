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

        [Test]
        public void AddingModelWithDictionaryOfStringObjectPairToDbAndCheckingStoredData()
        {
            var formClassName = "Form";
            var clusterid = _database.Create.Class(formClassName).Run();
            var form = new Form();
            form.Type = "Test Form";
            form.ORID = null;
            //form.FormData = new Dictionary<string, object>();
            ////adding string to dictionary
            //form.FormData.Add("String Test1", "Andrew");
            //form.FormData.Add("String Test2", "Jack");
            ////adding Guid to dictionary
            //form.FormData.Add("Guid Test1", Guid.NewGuid());
            //form.FormData.Add("Guid Test2", Guid.NewGuid());
            ////adding Number to dictionary
            //form.FormData.Add("Number Test1", 2);
            //form.FormData.Add("Number Test2", 4);
            ////adding Bool to dictionary
            //form.FormData.Add("Bool Test1", false);
            //form.FormData.Add("Bool Test2", true);
            ////adding DateTime to dictionary
            //form.FormData.Add("DateTime Test1", DateTime.Now);
            //form.FormData.Add("DateTime Test2", DateTime.UtcNow);

            var dateTimeNow = DateTime.Now;
            // OrientDB truncates milliseconds, so do so here for a proper comparison
            dateTimeNow = dateTimeNow.AddTicks(-(dateTimeNow.Ticks % TimeSpan.TicksPerSecond));
            
            var dateTimeUtcNow = DateTime.UtcNow;
            // OrientDB truncates milliseconds, so do so here for a proper comparison
            dateTimeUtcNow = dateTimeUtcNow.AddTicks(-(dateTimeUtcNow.Ticks % TimeSpan.TicksPerSecond));


            form.FormData = new ODocument();
            //adding string to dictionary
            form.FormData.SetField("String Test1", "Andrew");
            form.FormData.SetField("String Test2", "Jack");
            //adding Guid to dictionary
            form.FormData.SetField("Guid Test1", Guid.NewGuid());
            form.FormData.SetField("Guid Test2", Guid.NewGuid());
            //adding Number to dictionary
            form.FormData.SetField("Number Test1", 2);
            form.FormData.SetField("Number Test2", 4);
            //adding Bool to dictionary
            form.FormData.SetField("Bool Test1", false);
            form.FormData.SetField("Bool Test2", true);
            //adding DateTime to dictionary
            form.FormData.SetField("DateTime Test1", dateTimeNow);
            form.FormData.SetField("DateTime Test2", dateTimeUtcNow);
            //adding the document to db
            var document = _database.Insert(form).Into(formClassName).Run();

            var result = _database.Query<Form>("SELECT * FROM " + formClassName).SingleOrDefault();

            Assert.AreEqual(form.Type, result.Type);

            // testing the string datas
            Assert.True(result.FormData.ContainsKey("String Test1"));
            Assert.True(result.FormData.ContainsKey("String Test2"));
            Assert.AreEqual(form.FormData["String Test1"], result.FormData["String Test1"]);
            Assert.AreEqual(form.FormData["String Test2"], result.FormData["String Test2"]);

            // testing the Guid datas
            Assert.True(result.FormData.ContainsKey("Guid Test1"));
            Assert.True(result.FormData.ContainsKey("Guid Test2"));
            Assert.AreEqual(form.FormData["Guid Test1"], result.FormData.GetField<Guid>("Guid Test1"));
            Assert.AreEqual(form.FormData["Guid Test2"], result.FormData.GetField<Guid>("Guid Test2"));

            // testing the Number datas
            Assert.True(result.FormData.ContainsKey("Number Test1"));
            Assert.True(result.FormData.ContainsKey("Number Test2"));
            Assert.AreEqual(form.FormData["Number Test1"], result.FormData.GetField<int>("Number Test1"));
            Assert.AreEqual(form.FormData["Number Test2"], result.FormData.GetField<int>("Number Test2"));

            // testing the Bool datas
            Assert.True(result.FormData.ContainsKey("Bool Test1"));
            Assert.True(result.FormData.ContainsKey("Bool Test2"));
            Assert.AreEqual(form.FormData["Bool Test1"], result.FormData.GetField<bool>("Bool Test1"));
            Assert.AreEqual(form.FormData["Bool Test2"], result.FormData.GetField<bool>("Bool Test2"));

            // testing the Datetime datas
            Assert.True(result.FormData.ContainsKey("DateTime Test1"));
            Assert.True(result.FormData.ContainsKey("DateTime Test2"));
            Assert.AreEqual(form.FormData["DateTime Test1"], result.FormData.GetField<DateTime>("DateTime Test1"));
            Assert.AreEqual(form.FormData["DateTime Test2"], result.FormData.GetField<DateTime>("DateTime Test2"));
        }

        public class Form
        {
            public ORID ORID { get; set; }
            public string Type { get; set; }
            public ODocument FormData { get; set; }
        }

        public class Person
        {
            public string Name { get; set; }
            public Dictionary<string, int> EmbededMap { get; set; }
        }
    }
}
