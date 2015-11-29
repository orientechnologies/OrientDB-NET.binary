using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class TestCreateVertex
    {
        [Fact]
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
