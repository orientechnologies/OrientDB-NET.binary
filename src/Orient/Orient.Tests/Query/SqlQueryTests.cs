using System;
using Xunit;
using Orient.Client;
using System.Collections.Generic;
using System.Linq;

namespace Orient.Tests.Query
{
    
    public class SqlQueryTests
    {
        [Fact]
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
                Assert.Equal(result[0].Name, "Shawn");
                Assert.Equal(result[0].Age, 37);
            }
        }

        class TestVertex : OBaseRecord
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }

        [Fact]
        public void ShouldFetchLinkedDocumentsFromSimpleQuery()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Create.Class("Owner").Extends("V").Run();
                database.Create.Class("Computer").Extends("V").Run();
                var owner = new ODocument { OClassName = "Owner" };

                owner.SetField<String>("name", "Shawn");

                owner = database.Create.Vertex(owner).Run();

                var computer = new ODocument { OClassName = "Computer" };

                computer.SetField<ORID>("owner", owner.ORID);
                database.Create.Vertex(computer).Run();

                computer = database.Query("SELECT FROM Computer", "*:-1").FirstOrDefault();

                Assert.True(database.ClientCache.ContainsKey(computer.GetField<ORID>("owner")));

                var document = database.ClientCache[computer.GetField<ORID>("owner")];
                Assert.Equal(document.GetField<string>("name"), "Shawn");
            }
        }

        [Fact]
        public void ShouldFetchDocumentsConnectedByEdges()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // Need to set this 
                    database.Command("alter database custom useLightweightEdges=true");

                    database.Create.Class("House").Extends("V").Run();

                    database.Create.Class("Person").Extends("V").Run();
                    database.Create.Class("Owns").Extends("E").Run();

                    var house = new ODocument { OClassName = "House" };
                    database.Create.Vertex(house).Run();

                    var person = new ODocument { OClassName = "Person" };
                    person.SetField("name", "Shawn");
                    database.Create.Vertex(person).Run();

                    var person1 = new ODocument { OClassName = "Person" };
                    person1.SetField("name", "Roman");
                    database.Create.Vertex(person1).Run();

                    database.Create.Edge("Owns").From(person).To(house).Run();
                    database.Create.Edge("Owns").From(person1).To(house).Run();

                    house = database.Query("select from House", "*:-1").FirstOrDefault();

                    Assert.True(house != null);

                    Assert.Equal(database.ClientCache.Count(), 2); // client cache should contain two records;
                }
            }
        }
    }
}

