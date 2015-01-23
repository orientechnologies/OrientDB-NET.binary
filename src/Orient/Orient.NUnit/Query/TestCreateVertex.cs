using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestFixture]
    public class TestCreateVertex
    {
        [Test]
        public void ShouldCreateVertex()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                //var document = database.Command("create vertex set  bar=1").ToSingle();
                var d = database.Create.Vertex<OVertex>().Run();
            }
        }
    }
}
