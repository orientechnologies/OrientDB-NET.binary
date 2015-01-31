using System;
using NUnit.Framework;
using Orient.Client;
using System.Collections.Generic;

namespace Orient.Tests
{
    [TestFixture]
    public class SqlQueryDeserializeTests
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
    }
}

