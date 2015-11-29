using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Pool
{
    
    public class ConnectionPoolTests
    {
        [Fact]
        public void ShouldRetrieveAndReturnDatabaseFromPool()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                Assert.Equal(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    Assert.Equal(
                        TestConnection.GlobalTestDatabasePoolSize - 1,
                        OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                    );
                }

                Assert.Equal(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );
            }
        }

        [Fact]
        public void ShouldReturnDatabaseToPoolAfterCloseAndDisposeCall()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                Assert.Equal(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );

                ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);

                Assert.Equal(
                    TestConnection.GlobalTestDatabasePoolSize - 1,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );

                database.Close();

                Assert.Equal(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );

                database.Dispose();

                Assert.Equal(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );
            }
        }
    }
}
