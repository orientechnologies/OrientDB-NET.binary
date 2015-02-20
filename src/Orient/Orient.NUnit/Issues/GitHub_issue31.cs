using System;
using NUnit.Framework;
using Orient.Client;
using System.Linq;

namespace Orient.Tests.Issues
{
    [TestFixture]
    public class GitHub_issue31
    {
        [Test]
        public void ProblemDeseralizingObjectWithDateTimeFields_31()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var startRecords = database.CountRecords;

                // explicitly setting the timezone to UTC instead of the JVM default timezone
                // FIXME: this is a work around for now
                database.Command("ALTER DATABASE TIMEZONE UTC");
                database.Create.Class<File>().Extends<OVertex>().CreateProperties().Run();

                var dateTime = DateTime.UtcNow;
                // OrientDB truncates milliseconds, so do so here for a proper comparison
                dateTime = dateTime.AddTicks( - (dateTime.Ticks % TimeSpan.TicksPerSecond));

                database.Insert(new File
                    {
                        Filename = "myfile",
                        Created = dateTime
                    }).Run();

                var doc = database.Select().From<File>().ToList().First();
                var file = doc.To<File>();

                // FIXME: the time zone is off
                Assert.That(file.Created, Is.EqualTo(dateTime));

                var endRecords = database.CountRecords;
                Assert.AreEqual(startRecords + 1, endRecords);
            }

        }

        class File : OBaseRecord
        {
            public String Filename { get; set; }

            public DateTime Created { get; set; }
        }
    }
}

