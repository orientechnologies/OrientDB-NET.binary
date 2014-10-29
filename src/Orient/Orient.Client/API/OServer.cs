using System;
using System.Collections.Generic;
using Orient.Client.Protocol;
using Orient.Client.Protocol.Operations;

namespace Orient.Client
{
    public class OServer : IDisposable
    {
        private Connection _connection;

        public OServer(string hostname, int port, string userName, string userPassword)
        {
            _connection = new Connection(hostname, port, userName, userPassword);
        }

        public bool CreateDatabase(string databaseName, ODatabaseType databaseType, OStorageType storageType)
        {
            DbCreate operation = new DbCreate(null);
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.StorageType = storageType;

            ODocument document = _connection.ExecuteOperation(operation);

            return document.GetField<bool>("IsCreated");
        }

        public bool DatabaseExist(string databaseName, OStorageType storageType)
        {
            DbExist operation = new DbExist(null);
            operation.DatabaseName = databaseName;
            operation.StorageType = storageType;

            ODocument document = _connection.ExecuteOperation(operation);

            return document.GetField<bool>("Exists");
        }

        public void DropDatabase(string databaseName, OStorageType storageType)
        {
            DbDrop operation = new DbDrop(null);
            operation.DatabaseName = databaseName;
            operation.StorageType = storageType;

            ODocument document = _connection.ExecuteOperation(operation);
        }

        #region Configuration

        public string ConfigGet(string key)
        {
            ConfigGet operation = new ConfigGet(null);
            operation.ConfigKey = key;
            ODocument document = _connection.ExecuteOperation(operation);
            return document.GetField<string>(key);
        }

        public bool ConfigSet(string configKey, string configValue)
        {
            ConfigSet operation = new ConfigSet(null);
            operation.Key = configKey;
            operation.Value = configValue;
            ODocument document = _connection.ExecuteOperation(operation);

            return document.GetField<bool>("IsCreated");
        }

        public Dictionary<string, string> ConfigList()
        {
            ConfigList operation = new ConfigList(null);
            ODocument document = _connection.ExecuteOperation(operation);
            return document.GetField<Dictionary<string, string>>("config");
        }

        #endregion

        public Dictionary<string, string> Databases()
        {
            Dictionary<string, string> returnValue = new Dictionary<string, string>();
            DBList operation = new DBList(null);
            ODocument document = _connection.ExecuteOperation(operation);
            if (OClient.Serializer == API.Types.ORecordFormat.ORecordDocument2csv)
            {
                string[] databases = document.GetField<string>("databases").Split(',');
                foreach (var item in databases)
                {
                    string[] keyValue = item.Split(new char[] { ':' }, 2);
                    returnValue.Add(keyValue[0].Replace("\"", ""), keyValue[1].Replace("\"", ""));
                }
            }
            else
            {
                var databases = document.GetField<Dictionary<string, object>>("databases");
                foreach (var item in databases)
                {
                    returnValue.Add(item.Key, item.Value.ToString());
                }
            }
            return returnValue;
        }


        public void Close()
        {
            _connection.Dispose();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
