using System;

namespace Orient.Nunit.Test
{
    public class TestDatabaseContext : IDisposable
    {
        public TestDatabaseContext()
        {
            TestConnection.CreateTestDatabase();
        }

        public void Dispose()
        {
            TestConnection.DropTestDatabase();
        }
    }
}