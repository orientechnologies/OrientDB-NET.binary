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

            ODocument document = _connection.ExecuteOperation<DbCreate>(operation);

            return document.GetField<bool>("IsCreated");
        }

        public bool DatabaseExist(string databaseName, OStorageType storageType)
        {
            DbExist operation = new DbExist();
            operation.DatabaseName = databaseName;
            operation.StorageType = storageType;

            ODocument document = _connection.ExecuteOperation<DbExist>(operation);

            return document.GetField<bool>("Exists");
        }

        public void DropDatabase(string databaseName, OStorageType storageType)
        {
            DbDrop operation = new DbDrop();
            operation.DatabaseName = databaseName;
            operation.StorageType = storageType;

            ODocument document = _connection.ExecuteOperation<DbDrop>(operation);
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
