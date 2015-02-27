using System;
using System.Text;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Database
{
    [TestFixture(Category="Database opperations")]
    public class TestDatabaseOperations
    {
        [Test]
        public void ShouldReturnDatabaseSize()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var size = database.Size;
                Assert.IsTrue(size > 0);
            }
        }

        [Test]
        public void ShouldRetrieveCountRecords()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var startRecords = database.CountRecords;
                database
                    .Create
                    .Vertex("V")
                    .Set("bar", 1)
                    .Run();

                var endRecords = database.CountRecords;
                Assert.AreEqual(startRecords + 1, endRecords);
            }
        }

        [Test]
        public void ShouldLoadDatabaseProperties()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var document = database.DatabaseProperties;
                Assert.That(document, Is.Not.Null);
                Assert.That(document.Keys.Count, Is.GreaterThan(0));
            }
        }
    }
}
