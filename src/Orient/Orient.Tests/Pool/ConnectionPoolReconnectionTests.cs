using System;
using Xunit;
using Orient.Client;
using System.Threading.Tasks;

namespace Orient.Tests.Pool
{
    
    public class ConnectionPoolReconnectionTests
    {
        [Fact]
        public void ShouldConnectThroughTcpProxy()
        {
            using (var tcpProxy = new TcpProxy(12424))
            using (var context = new TestDatabaseContextProxy(tcpProxy.Forwarding))
            using (var database = new ODatabase(TestConnectionProxy.GlobalTestDatabaseAlias))
            {
                SimpleTest(database);
            }
        }

        [Fact]
        public void ShouldReconnectIfTcpProxyIsRestored()
        {
            using (var tcpProxy = new TcpProxy(12424))
            using (var context = new TestDatabaseContextProxy(tcpProxy.Forwarding))
            using (var database = new ODatabase(TestConnectionProxy.GlobalTestDatabaseAlias))
            {
                tcpProxy.End();
                tcpProxy.Start();

                try
                {
                    SimpleTest(database);
                    Assert.True(false, "SimpleTest should of failed without a socket connection");
                }
                catch
                {
                    SimpleTest(database);
                }
            }
        }

        private void SimpleTest(ODatabase database)
        {
            var startRecords = database.CountRecords;
            database
                .Create
                .Vertex("V")
                .Set("bar", 1)
                .Run();

            var endRecords = database.CountRecords;
            Assert.Equal(startRecords + 1, endRecords);
        }
    }
}

