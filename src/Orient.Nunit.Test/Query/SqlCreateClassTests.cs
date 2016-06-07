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
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short classId1 = database
                        .Create.Class("TestClass1")
                        .Run();

                    Assert.IsTrue(classId1 > 0);

                    short classId2 = database
                        .Create.Class("TestClass2")
                        .Run();

                    Assert.AreEqual(classId2, classId1 + 1);
                }
            }
        }

        [Test]
        public void ShouldCreateClassExtends()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short classId1 = database
                        .Create.Class("TestClass1")
                        .Extends("OVertex")
                        .Run();

                    Assert.IsTrue(classId1 > 0);

                    short classId2 = database
                        .Create.Class("TestClass2")
                        .Extends<OVertex>()
                        .Run();

                    Assert.AreEqual(classId2, classId1 + 1);
                }
            }
        }

        
        [Ignore("Broken Tests as at Github 27594c0114cd9489b69c84fe4896a9d6c6d01b19")]
        [Test]
        public void ShouldCreateClassCluster()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short classId1 = database
                        .Create.Class("TestClass1")
                        .Cluster(6)
                        .Run();

                    Assert.IsTrue(classId1 > 0);

                    short classId2 = database
                        .Create.Class("TestClass2")
                        .Cluster(6)
                        .Run();

                    Assert.AreEqual(classId2, classId1 + 1);
                }
            }
        }
    }
}
