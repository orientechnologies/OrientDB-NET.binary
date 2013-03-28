using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Server
{
    [TestClass]
    public class ServerOperationsTests
    {
        [TestMethod]
        public void ShouldCreateAndDeleteDatabase()
        {
            string databaseName = "thisIsTestDatabaseForNetDriver";
            OServer server = TestConnection.GetServer();

            bool exists = server.DatabaseExist(databaseName);

            Assert.AreEqual(exists, false);

            if (!exists)
            {
                bool isCreated = server.CreateDatabase(databaseName, ODatabaseType.Graph, OStorageType.Local);

                Assert.AreEqual(isCreated, true);

                if (isCreated)
                {
                    server.DropDatabase(databaseName);

                    exists = server.DatabaseExist(databaseName);

                    Assert.AreEqual(exists, false);
                }
            }
        }
    }
}
