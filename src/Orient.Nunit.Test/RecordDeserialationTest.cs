using System.Collections.Generic;
using NUnit.Framework;
using Orient.Client;
using Orient.Client.API.Types;

namespace Orient.Nunit.Test
{
    [TestFixture]
    public class ServerOperationsTests
    {
        [Test]
        public void ShouldCreateAndDeleteDatabase()
        {
            string databaseName = "thisIsTestDatabaseForNetDriver";
            OServer server = TestConnection.GetServer();

            bool exists = server.DatabaseExist(databaseName, OStorageType.PLocal);

            if (exists)
            {
                server.DropDatabase(databaseName, OStorageType.PLocal);

                exists = server.DatabaseExist(databaseName, OStorageType.PLocal);
            }

            Assert.AreEqual(exists, false);

            if (!exists)
            {
                bool isCreated = server.CreateDatabase(databaseName, ODatabaseType.Graph, OStorageType.PLocal);

                Assert.AreEqual(isCreated, true);

                if (isCreated)
                {
                    server.DropDatabase(databaseName, OStorageType.PLocal);

                    exists = server.DatabaseExist(databaseName, OStorageType.PLocal);

                    Assert.AreEqual(exists, false);
                }
            }
        }

        [Test]
        public void TestDbList()
        {
            OServer server = TestConnection.GetServer();
            Dictionary<string, ODatabaseInfo> databases = server.Databases();
            Assert.IsTrue(databases.Count > 0);
        }
    }
}
