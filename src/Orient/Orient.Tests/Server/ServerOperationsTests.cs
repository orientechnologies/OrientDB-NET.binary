using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Server
{
    [TestClass]
    public class ServerOperationsTests : IDisposable
    {
        public ServerOperationsTests()
        {
            TestConnection.CreateTestDatabase(
                TestConnection.GlobalTestDatabaseName,
                TestConnection.GlobalTestDatabaseType,
                TestConnection.GlobalTestDatabaseAlias
            );
        }

        [TestMethod]
        public void ShouldCreateAndDeleteDatabase()
        {
            string databaseName = "thisIsTestDatabaseForNetDriver";
            OServer server = TestConnection.GetServer();

            bool exists = server.DatabaseExist(databaseName);

            Assert.AreEqual(exists, false);

            if (!exists)
            {
                bool isCreated = server.CreateDatabase(databaseName, ODatabaseType.Graph, OStorageType.Remote);

                Assert.AreEqual(isCreated, true);

                if (isCreated)
                {
                    server.DropDatabase(databaseName);

                    exists = server.DatabaseExist(databaseName);

                    Assert.AreEqual(exists, false);
                }
            }
        }

        public void Dispose()
        {
            TestConnection.DropTestDatabase(TestConnection.GlobalTestDatabaseName);
        }
    }
}
