using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
    [TestFixture]
    public class SqlCreateClassTests
    {
        [Test]
        public void ShouldCreateClass()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    string classId1 = database
                        .Create.Class("TestClass1")
                        .Run();

                    Assert.IsTrue(classId1 == "TestClass1");
                    
                }
            }
        }

        [Test]
        public void ShouldCreateClassExtends()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    string classId1 = database
                        .Create.Class("TestClass1")
                        .Extends("OVertex")
                        .Run();

                    Assert.IsTrue(classId1 =="TestClass1");

                }
            }
        }

        
        [Ignore("Broken Tests as at Github 27594c0114cd9489b69c84fe4896a9d6c6d01b19")]
        [Test]
        public void ShouldCreateClassCluster()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.ConnectionOptions))
                {
                    string classId1 = database
                        .Create.Class("TestClass1")
                        .Cluster(6)
                        .Run();

                    Assert.IsTrue(classId1== "TestClass1");

                    
                }
            }
        }
    }
}
