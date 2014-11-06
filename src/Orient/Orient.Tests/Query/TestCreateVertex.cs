using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class TestCreateVertex
    {
        [TestMethod]
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
