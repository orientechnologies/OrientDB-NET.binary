using System;

namespace Orient.Nunit.Test
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