using System;

namespace Orient.Console
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
