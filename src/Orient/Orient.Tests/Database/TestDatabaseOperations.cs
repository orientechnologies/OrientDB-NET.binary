using System;
using System.Text;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Database
{
    public class TestDatabaseOperations
    {
        [Fact]
        [Trait("type", "Database")]
        public void ShouldReturnDatabaseSize()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var size = database.Size;
                Assert.True(size > 0);
            }
        }

        [Fact]
        [Trait("type", "Database")]
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
                Assert.Equal(startRecords + 1, endRecords);
            }
        }

        [Fact]
        [Trait("type", "Database")]
        public void ShouldLoadDatabaseProperties()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var document = database.DatabaseProperties;
                Assert.NotNull(document);
                Assert.True(document.Keys.Count > 0);
            }
        }
    }
}
