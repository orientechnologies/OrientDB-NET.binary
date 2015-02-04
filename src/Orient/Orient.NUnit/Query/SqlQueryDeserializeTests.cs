using System;
using NUnit.Framework;
using Orient.Client;
using System.Collections.Generic;
using System.Linq;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class SqlQueryTests
    {
        [Test]
        public void ShouldConvertSQLQueryResultsToClass()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Create.Class<TestVertex>().Run();
                database.Create.Vertex(new TestVertex
                    {
                        Name = "Shawn",
                        Age = 37
                    }).Run();
                List<TestVertex> result = database.Query<TestVertex>("SELECT FROM TestVertex");
                Assert.That(result[0].Name, Is.EqualTo("Shawn"));
                Assert.That(result[0].Age, Is.EqualTo(37));
            }
        }

        class TestVertex : OBaseRecord
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }

        [Test]
        public void ShouldFetchLinkedDocument()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Create.Class("Owner").Extends("V").Run();
                database.Create.Class("Place").Extends("V").Run();
                database.Create.Property("Place.owner", OType.Link).LinkedClass("Owner");
                var owner = new ODocument
                {
                    OClassName = "Owner"
                };

                owner.SetField<String>("name", "Shawn");

                owner = database.Create.Vertex(owner).Run();
                
                var place = new ODocument
                {
                    OClassName = "Place"
                };

                place.SetField<string>("country", "France");
                place.SetField<ORID>("owner", owner.ORID);
                database.Create.Vertex(place).Run();

                place = database.Query("SELECT FROM Place", "*:-1").FirstOrDefault();
                
                Assert.That(database.ClientCache.ContainsKey(place.GetField<ORID>("owner")));
                
                // FIXME: cannot cast because "owner" contains only the ORID
                var document = database.ClientCache[place.GetField<ORID>("owner")];
                Assert.That(document.GetField<string>("name"), Is.EqualTo("Shawn"));
            }
        }
    }
}

