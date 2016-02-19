using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class SqlUpdateWithEmbeddedLinkedClass
    {
        [Test]
        public void ShouldUpdateRecordWithEmbeddedLinkedClass()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (var db = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                // Arrange
                if (!db.Schema.IsClassExist<TestEmbeddedClass>())
                {
                    db.Create.Class<TestEmbeddedClass>().CreateProperties<TestEmbeddedClass>().Run();
                }

                if (!db.Schema.IsClassExist<TestClass>())
                {
                    db.Create.Class<TestClass>().Run();
                    db.Create.Property("SomeString", OType.String).Class("TestClass").Run();
                    db.Create.Property("EmbeddedClass", OType.Embedded).Class("TestClass").LinkedClass("TestEmbeddedClass").Run();
                }

                var toInsert = new TestClass()
                {
                    SomeString = "apple",
                    EmbeddedClass = new TestEmbeddedClass() { StringProperty = "embeddedApple" },
                };

                var saved = db.Insert(toInsert).Run();

                saved.SetField("SomeString", "pear");
                saved.SetField("EmbeddedClass.StringProperty", "embeddedPear");

                // Act
                db.Transaction.Update(saved);
                db.Transaction.Commit();

                // Assert
                var updated = db.Select().From(saved.ORID).ToList().Single();

                Assert.AreEqual("pear", saved.GetField<string>("SomeString"));
                Assert.AreEqual("embeddedPear", saved.GetField<string>("EmbeddedClass.StringProperty"));
            }
        }

        public class TestClass
        {
            public string SomeString { get; set; }
            public TestEmbeddedClass EmbeddedClass { get; set; }
        }

        public class TestEmbeddedClass
        {
            public string StringProperty { get; set; }
        }
    }
}
