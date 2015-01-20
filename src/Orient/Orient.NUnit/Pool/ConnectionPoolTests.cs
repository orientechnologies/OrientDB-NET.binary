using System;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Pool
{
    [TestFixture]
    public class ConnectionPoolTests
    {
        [Test]
        public void ShouldRetrieveAndReturnDatabaseFromPool()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                Assert.AreEqual(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    Assert.AreEqual(
                        TestConnection.GlobalTestDatabasePoolSize - 1,
                        OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                    );
                }

                Assert.AreEqual(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );
            }
        }

        [Test]
        public void ShouldReturnDatabaseToPoolAfterCloseAndDisposeCall()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                Assert.AreEqual(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );

                ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);

                Assert.AreEqual(
                    TestConnection.GlobalTestDatabasePoolSize - 1,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );

                database.Close();

                Assert.AreEqual(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );

                database.Dispose();

                Assert.AreEqual(
                    TestConnection.GlobalTestDatabasePoolSize,
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias)
                );
            }
        }
    }
}
