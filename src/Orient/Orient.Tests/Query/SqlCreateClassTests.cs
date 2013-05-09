using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlCreateClassTests
    {
        [TestMethod]
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

        [TestMethod]
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

        [TestMethod]
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
