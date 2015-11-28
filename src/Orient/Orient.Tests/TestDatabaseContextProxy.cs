using System;

namespace Orient.Tests
{
    public class TestDatabaseContextProxy : IDisposable
    {
        TestConnectionProxy connection;

        public TestDatabaseContextProxy(int port)
        {
            connection = new TestConnectionProxy(port);
            connection.CreateTestDatabase();
            connection.CreateTestPool();
        }

        public void Dispose()
        {
            connection.DropTestPool();
            connection.DropTestDatabase();
            connection.Dispose();
        }
    }
}
