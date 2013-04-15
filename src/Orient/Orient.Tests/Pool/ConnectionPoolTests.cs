using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Pool
{
    [TestClass]
    public class ConnectionPoolTests
    {
        [TestMethod]
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

        [TestMethod]
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
