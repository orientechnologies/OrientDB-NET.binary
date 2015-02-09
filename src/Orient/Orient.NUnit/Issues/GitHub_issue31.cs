using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture]
    public class GitHub_issue31
    {
        [Test]
        public void ProblemInsertingObjectWithDateTimeFields_31()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var startRecords = database.CountRecords;

                database.Create.Class<File>().Extends<OVertex>().CreateProperties().Run();
                database.Insert(new File
                    {
                        Filename = "myfile",
                        Created = DateTime.Now
                    }).Run();

                var endRecords = database.CountRecords;
                Assert.AreEqual(startRecords + 1, endRecords);
            }

        }

        class File
        {
            public String Filename { get; set; }

            public DateTime Created { get; set; }
        }
    }
}

