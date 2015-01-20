using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Database
{
    [TestFixture]
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
                    .Set("bar",1)
                    .Run();

                var endRecords = database.CountRecords;
                Assert.AreEqual(startRecords + 1,endRecords);
            }
        }
    }
}
