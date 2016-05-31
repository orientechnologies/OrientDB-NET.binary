using NUnit.Framework;
using Orient.Client;
using Orient.Nunit.Test;

namespace Orient.Nunit.Test.Pool
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
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias), 
                    TestConnection.GlobalTestDatabasePoolSize
                );

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    Assert.AreEqual(
                        OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias),
                        TestConnection.GlobalTestDatabasePoolSize - 1
                    );
                }

                Assert.AreEqual(
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias), 
                    TestConnection.GlobalTestDatabasePoolSize
                );
            }
        }

        [Test]
        public void ShouldReturnDatabaseToPoolAfterCloseAndDisposeCall()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                Assert.AreEqual(
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias),
                    TestConnection.GlobalTestDatabasePoolSize
                );

                ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);

                Assert.AreEqual(
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias),
                    TestConnection.GlobalTestDatabasePoolSize - 1
                );

                database.Close();

                Assert.AreEqual(
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias),
                    TestConnection.GlobalTestDatabasePoolSize
                );

                database.Dispose();

                Assert.AreEqual(
                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias),
                    TestConnection.GlobalTestDatabasePoolSize
                );
            }
        }
    }
}
