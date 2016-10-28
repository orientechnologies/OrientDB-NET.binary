using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlCreateClusterTests
    {
        [Test]
        public void ShouldCreateCluster()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    short clusterId1 = database
                        .Create.Cluster("TestCluster1", OClusterType.Physical)
                        .Run();

                    Assert.IsTrue(clusterId1 > 0);

                    short clusterId2 = database
                        .Create.Cluster<TestProfileClass>(OClusterType.Physical)
                        .Run();

                    Assert.AreEqual(clusterId2, clusterId1 + 1);
                }
            }
        }
    }
}
