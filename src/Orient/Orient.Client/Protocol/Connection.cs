using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using Orient.Client.Protocol.Operations;
using Orient.Client.Protocol.Serializers;
using System.IO;

namespace Orient.Client.Protocol
{
    internal class Connection : IDisposable
    {
        private TcpClient _socket;
        private NetworkStream _networkStream;
        private byte[] _readBuffer;
        private int RECIVE_TIMEOUT = 30 * 1000; // Recive timeout in milliseconds
        private const int RetryCount = 3;

        internal string Hostname { get; set; }
        internal int Port { get; set; }
        internal ConnectionType Type { get; private set; }
        internal ODatabase Database { get; set; }

        internal string DatabaseName { get; private set; }
        internal ODatabaseType DatabaseType { get; private set; }
        internal string UserName { get; private set; }
        internal string UserPassword { get; private set; }

        internal string Alias { get; set; }
        internal bool IsReusable { get; set; }
        internal short ProtocolVersion { get; set; }

        internal int SessionId { get; private set; }

        private byte[] _serverToken;
        private byte[] _databaseToken;

        internal byte[] Token
        {
            get
            {
                return (Type == ConnectionType.Database) ? _databaseToken : _serverToken;
            }
            set
            {
                if (Type == ConnectionType.Database)
                    _databaseToken = value;
                else
                    _serverToken = value;
            }
        }

        internal bool IsActive
        {
            get
            {
                // If the socket has been closed by your own actions (disposing the socket, 
                // calling methods to disconnect), Socket.Connected will return false. If 
                // the socket has been disconnected by other means, the property will return 
                // true until the next attempt to send or receive information.
                // more info: http://stackoverflow.com/questions/2661764/how-to-check-if-a-socket-is-connected-disconnected-in-c
                // why not to use socket.Poll solution: it fails when the socket is being initialized
                // and introduces additional delay for connection check
                if ((Socket != null) && Socket.Connected)
                {
                    return true;
                }

                return false;
            }
        }
        internal ODocument Document { get; set; }

        internal bool UseTokenBasedSession { get; set; }

        public TcpClient Socket
        {
            get
            {
                return _socket;
            }

            set
            {
                _socket = value;
            }
        }

        internal Connection(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, string alias, bool isReusable)
        {
            Hostname = hostname;
            Port = port;
            Type = ConnectionType.Database;
            Alias = alias;
            IsReusable = isReusable;
            ProtocolVersion = 0;
            SessionId = -1;
            UseTokenBasedSession = OClient.UseTokenBasedSession;

            DatabaseName = databaseName;
            DatabaseType = databaseType;
            UserName = userName;
            UserPassword = userPassword;
            InitializeDatabaseConnection(databaseName, databaseType, userName, userPassword);
        }

        internal Connection(string hostname, int port, string userName, string userPassword)
        {
            Hostname = hostname;
            Port = port;
            Type = ConnectionType.Server;
            IsReusable = false;
            ProtocolVersion = 0;
            SessionId = -1;
            UseTokenBasedSession = OClient.UseTokenBasedSession;

            UserName = userName;
            UserPassword = userPassword;

            InitializeServerConnection(userName, userPassword);
        }

        internal ODocument ExecuteOperation(IOperation operation)
        {
            Exception _lastException = null;

            var i = RetryCount;
            while (i-- > 0)
            {
                try
                {
                    return ExecuteOperationInternal(operation);
                }
                catch (IOException ex)
                {
                    if (_lastException == null)
                        _lastException = ex;
                    else
                        _lastException = new Exception("Retry patern exception", ex);
                }
            }

            throw _lastException;
        }

        private ODocument ExecuteOperationInternal(IOperation operation)
        {
            try
            {
                if (_networkStream == null)
                    Reconnect();

                var req = new Request(this);
                req.SetSessionId(SessionId);

                Request request = operation.Request(req);
                byte[] buffer;

                foreach (RequestDataItem item in request.DataItems)
                {
                    switch (item.Type)
                    {
                        case "byte":
                        case "short":
                        case "int":
                        case "long":
                            Send(item.Data);
                            break;
                        case "record":
                            buffer = new byte[2 + item.Data.Length];
                            Buffer.BlockCopy(BinarySerializer.ToArray(item.Data.Length), 0, buffer, 0, 2);
                            Buffer.BlockCopy(item.Data, 0, buffer, 2, item.Data.Length);
                            Send(buffer);
                            break;
                        case "bytes":
                        case "string":
                        case "strings":
                            Send(BinarySerializer.ToArray(item.Data.Length));
                            Send(item.Data);
                            break;
                        default:
                            break;
                    }
                }

                _networkStream.Flush();

                if (request.OperationMode != OperationMode.Synchronous)
                    return null;

                Response response = new Response(this);
                response.Receive();
                return ((IOperation)operation).Response(response);
            }
            catch (IOException)
            {
                Destroy();
                throw;
            }
        }

        private void Reconnect()
        {
            Close();
            if (Type == ConnectionType.Database)
            {
                InitializeDatabaseConnection(DatabaseName, DatabaseType, UserName, UserPassword);
            }
            else
            {
                InitializeServerConnection(UserName, UserPassword);
            }
        }

        internal Stream GetNetworkStream()
        {
            return _networkStream;
        }

        internal void Destroy()
        {
            SessionId = -1;
            _databaseToken = null;
            _serverToken = null;

            try
            {
                if ((_networkStream != null) && (Socket != null))
                {
                    _networkStream.Dispose();
                    
                    #if NET451
                            Socket.Close();
                    #endif                        
                    
                    #if DOTNET5_5
                            // whatever 
                            Socket.Dispose();
                    #endif
                }
            }
            catch { }
            finally
            {
                _networkStream = null;
                Socket = null;
            }
        }

        internal void Close()
        {
            if (!IsActive)
                return;
            try
            {
                DbClose operation = new DbClose(this.Database);
                ExecuteOperation(operation);
            }
            catch { }
            finally
            {
                Destroy();
            }
        }

        public void Dispose()
        {
            Close();
        }

        public void Reload()
        {
            DbReload operation = new DbReload(Database);
            var document = ExecuteOperation(operation);
            Document.SetField("Clusters", document.GetField<List<OCluster>>("Clusters"));
            Document.SetField("ClusterCount", document.GetField<short>("ClusterCount"));
        }

        #region Private methods

        private void InitializeDatabaseConnection(string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            _readBuffer = new byte[OClient.BufferLenght];

            // initiate socket connection
            try
            {
                var client = new TcpClient();

                Socket = new TcpClient();
                Socket.ReceiveTimeout = RECIVE_TIMEOUT;
                Socket.ConnectAsync(Hostname, Port).GetAwaiter().GetResult();
            }
            catch (SocketException ex)
            {
                throw new OException(OExceptionType.Connection, ex.Message, ex.InnerException);
            }

            _networkStream = Socket.GetStream();
            _networkStream.Read(_readBuffer, 0, 2);

            OClient.ProtocolVersion = ProtocolVersion = BinarySerializer.ToShort(_readBuffer.Take(2).ToArray());
            if (ProtocolVersion < 27)
                UseTokenBasedSession = false;

            // execute db_open operation
            DbOpen operation = new DbOpen(null);
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            Document = ExecuteOperation(operation);
            SessionId = Document.GetField<int>("SessionId");
            _databaseToken = Document.GetField<byte[]>("Token");
        }

        private void InitializeServerConnection(string userName, string userPassword)
        {
            _readBuffer = new byte[OClient.BufferLenght];

            // initiate socket connection
            try
            {
                Socket = new TcpClient();
                Socket.ReceiveTimeout = RECIVE_TIMEOUT;
                Socket.ConnectAsync(Hostname, Port).GetAwaiter().GetResult();
            }
            catch (SocketException ex)
            {
                throw new OException(OExceptionType.Connection, ex.Message, ex.InnerException);
            }

            _networkStream = Socket.GetStream();
            _networkStream.Read(_readBuffer, 0, 2);

            OClient.ProtocolVersion = ProtocolVersion = BinarySerializer.ToShort(_readBuffer.Take(2).ToArray());
            if (ProtocolVersion < 27)
                UseTokenBasedSession = false;

            if (ProtocolVersion <= 0)
                throw new OException(OExceptionType.Connection, "Incorect Protocol Version " + ProtocolVersion);

            // execute connect operation
            Connect operation = new Connect(null);
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            Document = ExecuteOperation(operation);
            SessionId = Document.GetField<int>("SessionId");
            _serverToken = Document.GetField<byte[]>("Token");
        }

        private void Send(byte[] rawData)
        {
            if ((_networkStream != null) && _networkStream.CanWrite)
            {
                try
                {
                    _networkStream.Write(rawData, 0, rawData.Length);
                }
                catch (Exception ex)
                {
                    throw new OException(OExceptionType.Connection, ex.Message, ex.InnerException);
                }
            }
        }

        #endregion
    }
}
