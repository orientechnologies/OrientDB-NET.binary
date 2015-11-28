using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Database
{
    
    public class DataClusterTest
    {
        [Fact]
        public void ShouldRetrieveNumberRecordsInSingleCluster()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Create.Class("TestDocumentClass").Run();

                for (int i = 0; i < 100; i++)
                {
                    database
                        .Create
                        .Document("TestDocumentClass")
                        .Set("bar", i)
                        .Run();
                }

                long recordsInCluster = database
                    .Clusters("TestDocumentClass")
                    .Count();

                Assert.Equal(100, recordsInCluster);
            }
        }

        [Fact]
        public void ShouldRetrieveDataRangeForSingle()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var document = database.Clusters("OUser").Range();
                Assert.NotNull(document);
                Assert.Equal(1, document.Count);
                Assert.True(document.ContainsKey("OUser"));
                
                var document1 = database.Clusters(4).Range();
                Assert.NotNull(document1);
                Assert.Equal(1, document1.Count);
                Assert.True(document1.ContainsKey("4"));
            }
        }

        [Fact]
        public void ShouldRetrieveDataRangeForMany()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var document = database.Clusters("OUser","ORole","V").Range();
                Assert.NotNull(document);
                Assert.Equal(3, document.Count);
                Assert.True(document.ContainsKey("OUser"));
                Assert.True(document.ContainsKey("ORole"));
                Assert.True(document.ContainsKey("V"));
                
                var document1 = database.Clusters(4, 5, 9).Range();
                Assert.NotNull(document1);
                Assert.Equal(3, document1.Count);
                Assert.True(document1.ContainsKey("4"));
                Assert.True(document1.ContainsKey("5"));
                Assert.True(document1.ContainsKey("9"));

            }
        }
    }
}
