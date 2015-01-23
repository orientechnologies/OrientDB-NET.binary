using System;

namespace Orient.Tests
{
    public class TestDatabaseContext : IDisposable
    {
        public TestDatabaseContext()
        {
            TestConnection.CreateTestDatabase();
            TestConnection.CreateTestPool();
        }

        public void Dispose()
        {
            TestConnection.DropTestPool();
            TestConnection.DropTestDatabase();
        }
    }
}
