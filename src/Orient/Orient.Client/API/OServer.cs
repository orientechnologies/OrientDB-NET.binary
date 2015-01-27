using System;
using System.Collections.Generic;
using Orient.Client.API.Types;
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

        public Dictionary<string, ODatabaseInfo> Databases()
        {
            Dictionary<string, ODatabaseInfo> returnValue = new Dictionary<string, ODatabaseInfo>();
            DBList operation = new DBList(null);
            ODocument document = _connection.ExecuteOperation(operation);
            ODocument databases = document.GetField<ODocument>("databases");
            if (OClient.Serializer == ORecordFormat.ORecordDocument2csv)
            {
                foreach (var item in databases)
                {
                    string databaseName = item.Key;
                    string[] pathValue = item.Value.ToString().Split(':');
                    var info = new ODatabaseInfo();
                    OStorageType storageType;

                    Enum.TryParse<OStorageType>(pathValue[0], true, out storageType);
                    info.DataBaseName = databaseName.Replace("\"", "");
                    info.StorageType = storageType;
                    info.Path = pathValue[1].Replace("\"", "");;
                    returnValue.Add(info.DataBaseName, info);
                }
            }
            else
            {
                foreach (var item in databases)
                {
                    var info = new ODatabaseInfo();
                    string[] pathValue = item.Value.ToString().Split(':');
                    OStorageType storageType;

                    Enum.TryParse<OStorageType>(pathValue[0], true, out storageType);
                    info.DataBaseName = item.Key;
                    info.StorageType = storageType;
                    info.Path = item.Value.ToString();
                    returnValue.Add(info.DataBaseName, info);
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
