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

        [TestCategory("Broken Tests as at Github 27594c0114cd9489b69c84fe4896a9d6c6d01b19")]
        [Ignore]
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

        class TestClass
        {
            public int Value { get; set; }
            public string Text { get; set; }
            public bool Flag { get; set; }
            public long LongValue { get; set; }
            public short ShortValue { get; set; }
            public byte Byte { get; set; }
            public byte[] data { get; set; }
        }

        [TestMethod]
        public void ShouldCreateClassWithProperties()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    short classId1 = database.Create.Class<TestClass>().CreateProperties().Run();

                    Assert.IsTrue(classId1 > 0);

                    // would like to test the properties have been created properly, but the server doesn't implement 'info' over the remove protocol 
                    //var result = database.Command("info class TestClass");
                    
                }
            }
        }
    }
}
