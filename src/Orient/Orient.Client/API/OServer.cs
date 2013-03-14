using System;
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
            DbCreate operation = new DbCreate();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.StorageType = storageType;

            DataObject dataObject = _connection.ExecuteOperation<DbCreate>(operation);

            return dataObject.Get<bool>("IsCreated");
        }

        public bool DatabaseExist(string databaseName)
        {
            DbExist operation = new DbExist();
            operation.DatabaseName = databaseName;

            DataObject dataObject = _connection.ExecuteOperation<DbExist>(operation);

            return dataObject.Get<bool>("Exists");
        }

        public void DeleteDatabase(string databaseName)
        {
            DbDrop operation = new DbDrop();
            operation.DatabaseName = databaseName;

            DataObject dataObject = _connection.ExecuteOperation<DbDrop>(operation);
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
