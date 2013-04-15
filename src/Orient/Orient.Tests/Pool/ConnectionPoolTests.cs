using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Pool
{
    [TestClass]
    public class ConnectionPoolTests
    {
        [TestMethod]
        public void ShouldCreateConnectionPool()
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
    }
}
