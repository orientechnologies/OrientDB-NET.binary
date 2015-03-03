using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture(Category = "issues")]
    public class GitHub_issue38
    {
        [Test]
        public void ShouldCreateRecordContainingSingleQuote()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (var db = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                if (!db.Schema.IsClassExist<TestClass>())
                {
                    db.Create.Class<TestClass>().CreateProperties<TestClass>().Run();
                }

                db.Command("delete from TestClass");

                var text = @"Jim'n";
                var test = new TestClass() { SomeString = text };
                db.Insert(test).Run();

                var result = db.Select().From<TestClass>().ToList<TestClass>();
                Assert.AreEqual(text, result.Single().SomeString);
            }
        }

        public class TestClass
        {
            public TestClass()
            {

            }
            public string SomeString { get; set; }
        }
    }
}
