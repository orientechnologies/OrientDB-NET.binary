using System;
using System.Collections.Generic;
using Xunit;
using Orient.Client;
using Orient.Client.API.Types;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Server
{
    
    public class ServerOperationsTests
    {
        [Fact]
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

            Assert.Equal(exists, false);

            if (!exists)
            {
                bool isCreated = server.CreateDatabase(databaseName, ODatabaseType.Graph, OStorageType.PLocal);

                Assert.Equal(isCreated, true);

                if (isCreated)
                {
                    server.DropDatabase(databaseName, OStorageType.PLocal);

                    exists = server.DatabaseExist(databaseName, OStorageType.PLocal);

                    Assert.Equal(exists, false);
                }
            }
        }

        [Fact]
        public void TestDbList()
        {
            using (var context = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                OServer server = TestConnection.GetServer();
                Dictionary<string, ODatabaseInfo> databases = server.Databases();
                Assert.True(databases.Count > 0);
            }
        }
    }
}
