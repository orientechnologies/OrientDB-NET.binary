﻿using Orient.Client;
using System;

namespace Orient.Tests
{
    public class TestConnectionProxy : IDisposable
    {
        private static string _hostname = "127.0.0.1";
        private int port;
        private static string _username = "admin";
        private static string _password = "admin";

        private static string _rootUserName = "root";
        private static string _rootUserParssword = "root";

        public static int GlobalTestDatabasePoolSize { get { return 3; } }
        public static string GlobalTestDatabaseName { get; private set; }
        public static ODatabaseType GlobalTestDatabaseType { get; private set; }
        public static string GlobalTestDatabaseAlias { get; private set; }
        public static OStorageType GlobalTestDatabaseStorageType { get; private set; }

        private OServer _server;

        public TestConnectionProxy(int port)
        {
            this.port = port;
            _server = new OServer(_hostname, port, _rootUserName, _rootUserParssword);

            GlobalTestDatabaseName = "globalTestDatabaseForNetDriver001";
            GlobalTestDatabaseType = ODatabaseType.Graph;
            GlobalTestDatabaseAlias = "globalTestDatabaseForNetDriver001AliasProxy";
            GlobalTestDatabaseStorageType = TestConnection.GlobalTestDatabaseStorageType;
        }

        public void CreateTestDatabase()
        {
            DropTestDatabase();

            _server.CreateDatabase(GlobalTestDatabaseName, GlobalTestDatabaseType, GlobalTestDatabaseStorageType);
        }

        public void DropTestDatabase()
        {
            var retry = 2;
            do
            {
                try
                {
                    if (_server.DatabaseExist(GlobalTestDatabaseName, GlobalTestDatabaseStorageType))
                    {
                        _server.DropDatabase(GlobalTestDatabaseName, GlobalTestDatabaseStorageType);
                    }
                    break;
                }
                catch
                {
                    _server.Dispose();
                    _server = new OServer(_hostname, port, _rootUserName, _rootUserParssword);
                }
            }
            while (--retry > 0);
        }

        public void CreateTestPool()
        {
            OClient.CreateDatabasePool(
                _hostname,
                port,
                GlobalTestDatabaseName,
                GlobalTestDatabaseType,
                _username,
                _password,
                GlobalTestDatabasePoolSize,
                GlobalTestDatabaseAlias
            );
        }

        public void DropTestPool()
        {
            OClient.DropDatabasePool(GlobalTestDatabaseAlias);
        }

        public OServer GetServer()
        {
            return _server;
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
