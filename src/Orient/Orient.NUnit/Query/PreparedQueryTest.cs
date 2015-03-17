using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;
using Orient.Client.API.Query;

namespace Orient.Tests.Query
{
    [TestFixture(Category = "query")]
    class PreparedQueryTest
    {
        TestDatabaseContext _context;
        ODatabase _database;

        [SetUp]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            _database.Create.Class("Profile").Run();
            var properties = new[]{
                new {name="Michael",surname = "Blach"},
                new {name="Shantaram",surname = "Gaikwad"},
                new {name="Luca",surname = "Garulli"}
            };

            foreach (var item in properties)
            {
                _database.Insert()
                    .Into("Profile")
                    .Set("name", item.name)
                    .Set("surname", item.surname).Run();
            }
        }

        [TearDown]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Test]
        public void ShouldExeccutePreparedQueryByPossition()
        {
            var query = new PreparedQuery("select from Profile where name = ? and surname = ?");
            var docMichael = _database
                .Query(query)
                .Run("Michael", "Blach").SingleOrDefault();

            Assert.That(docMichael, Is.Not.Null);
            Assert.That(docMichael.GetField<string>("name"), Is.EqualTo("Michael"));
            Assert.That(docMichael.GetField<string>("surname"), Is.EqualTo("Blach"));

            var docLuca = _database
                .Query(query)
                .Run("Luca", "Garulli").SingleOrDefault();
            Assert.That(docLuca, Is.Not.Null);
            Assert.That(docLuca.GetField<string>("name"), Is.EqualTo("Luca"));
            Assert.That(docLuca.GetField<string>("surname"), Is.EqualTo("Garulli"));


        }

        [Test]
        public void ShouldExeccutePreparedQueryByName()
        {
            var query = new PreparedQuery("select from Profile where name = :name and surname = :surname");

            var docMichael = _database
                .Query(query)
                .Set("name", "Michael")
                .Set("surname", "Blach")
                .Run().SingleOrDefault();

            Assert.That(docMichael, Is.Not.Null);
            Assert.That(docMichael.GetField<string>("name"), Is.EqualTo("Michael"));
            Assert.That(docMichael.GetField<string>("surname"), Is.EqualTo("Blach"));

            var docLuca = _database
                .Query(query)
                .Set("name", "Luca")
                .Set("surname", "Garulli")
                .Run().SingleOrDefault();

            Assert.That(docLuca, Is.Not.Null);
            Assert.That(docLuca.GetField<string>("name"), Is.EqualTo("Luca"));
            Assert.That(docLuca.GetField<string>("surname"), Is.EqualTo("Garulli"));

        }

        [Test]
        public void ShouldCreateDocumentsWithPreparedCommandByName()
        {
            var createQuery = new PreparedCommand("insert into Profile set name = :name , surname = :surname");
            var values = new[]{ 
                new { Name="Pura",Surname="Shields"},
                new { Name="Deedra",Surname="Bonura"},
                new { Name="Foster",Surname="Coppin"},
                new { Name="Bradly",Surname="Sanzone"}
            };
            foreach (var item in values)
            {
                var createdDoc = _database.Command(createQuery)
                    .Set("name", item.Name)
                    .Set("surname", item.Surname)
                    .Run().ToSingle();

                Assert.That(createdDoc.ORID, Is.Not.Null);
                Assert.That(createdDoc.GetField<string>("name"), Is.EqualTo(item.Name));
                Assert.That(createdDoc.GetField<string>("surname"), Is.EqualTo(item.Surname));
            }
        }

        [Test]
        public void ShouldCreateDocumentsWithPreparedCommandByPossition()
        {
            var createQuery = new PreparedCommand("insert into Profile set name = ? , surname = ?");
            var values = new[]{ 
                new { Name="Pura",Surname="Shields"},
                new { Name="Deedra",Surname="Bonura"},
                new { Name="Foster",Surname="Coppin"},
                new { Name="Bradly",Surname="Sanzone"}
            };
            foreach (var item in values)
            {
                var createdDoc = _database.Command(createQuery)
                    .Run(item.Name, item.Surname).ToSingle();

                Assert.That(createdDoc.ORID, Is.Not.Null);
                Assert.That(createdDoc.GetField<string>("name"), Is.EqualTo(item.Name));
                Assert.That(createdDoc.GetField<string>("surname"), Is.EqualTo(item.Surname));
            }
        }
    }
}
