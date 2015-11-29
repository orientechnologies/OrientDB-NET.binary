using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Orient.Client;
using Orient.Client.API.Query;

namespace Orient.Tests.Query
{
    class PreparedQueryTest : IDisposable
    {
        TestDatabaseContext _context;
        ODatabase _database;
        
        public PreparedQueryTest()
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

        public void Dispose()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Fact]
        [Trait("type", "query")]
        public void ShouldExeccutePreparedQueryByPossition()
        {
            var query = new PreparedQuery("select from Profile where name = ? and surname = ?");
            var docMichael = _database
                .Query(query)
                .Run("Michael", "Blach").SingleOrDefault();

            Assert.NotNull(docMichael);
            Assert.Equal(docMichael.GetField<string>("name"), "Michael");
            Assert.Equal(docMichael.GetField<string>("surname"), "Blach");

            var docLuca = _database
                .Query(query)
                .Run("Luca", "Garulli").SingleOrDefault();
            Assert.NotNull(docLuca);
            Assert.Equal(docLuca.GetField<string>("name"), "Luca");
            Assert.Equal(docLuca.GetField<string>("surname"), "Garulli");


        }

        [Fact]
        [Trait("type", "query")]
        public void ShouldExeccutePreparedQueryByName()
        {
            var query = new PreparedQuery("select from Profile where name = :name and surname = :surname");

            var docMichael = _database
                .Query(query)
                .Set("name", "Michael")
                .Set("surname", "Blach")
                .Run().SingleOrDefault();

            Assert.NotNull(docMichael);
            Assert.Equal(docMichael.GetField<string>("name"), "Michael");
            Assert.Equal(docMichael.GetField<string>("surname"), "Blach");

            var docLuca = _database
                .Query(query)
                .Set("name", "Luca")
                .Set("surname", "Garulli")
                .Run().SingleOrDefault();

            Assert.NotNull(docLuca);
            Assert.Equal(docLuca.GetField<string>("name"), "Luca");
            Assert.Equal(docLuca.GetField<string>("surname"), "Garulli");

        }

        [Fact]
        [Trait("type", "query")]
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

                Assert.NotNull(createdDoc.ORID);
                Assert.Equal(createdDoc.GetField<string>("name"), item.Name);
                Assert.Equal(createdDoc.GetField<string>("surname"), item.Surname);
            }
        }

        [Fact]
        [Trait("type", "query")]
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

                Assert.NotNull(createdDoc.ORID);
                Assert.Equal(createdDoc.GetField<string>("name"), item.Name);
                Assert.Equal(createdDoc.GetField<string>("surname"), item.Surname);
            }
        }
    }
}
