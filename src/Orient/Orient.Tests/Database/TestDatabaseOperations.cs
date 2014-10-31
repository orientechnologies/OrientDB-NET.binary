using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Database
{
    [TestClass]
    public class TestDatabaseOperations
    {
        [TestMethod]
        public void ShouldReturnDatabaseSize()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var size = database.Size;
                Assert.IsTrue(size > 0);
            }
        }
    }
}
