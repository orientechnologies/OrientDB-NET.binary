using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture(Category = "example")]
    public class GitHub_issue57
    {
        TestDatabaseContext _context;
        ODatabase _database;

        [SetUp]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);

            _database.Create.Class("TestVertex").Extends<OVertex>().Run();
            _database.Create.Class("TestEdge").Extends<OEdge>().Run();

        }

        [TearDown]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Test]
        public void x()
        {
            // create three classes extending V
            var personClusterId = _database.Create.Class("Persons").Extends("V").Run();
            var cityClusterId = _database.Create.Class("Cities").Extends("V").Run();
            var carsClusterId = _database.Create.Class("Cars").Extends("V").Run();

            // create two edges extendin E
            var livesClusterId = _database.Create.Class("Lives").Extends("E").Run();
            var ownsClusterId = _database.Create.Class("Owns").Extends("E").Run();


            //inser sample data
            var persons = new[] { "Michael", "Roman", "Luca", "Nick", "Fredi" };
            foreach (var p in persons)
            {
                _database.Insert().Into("Persons")
                    .Set("Name", p)
                    .Run();
            }

            var cars = new[] { "Audi", "Subaru", "BMW", "Mazda" };
            foreach (var c in cars)
            {
                _database.Insert().Into("Cars")
                    .Set("Name", c)
                    .Run();
            }

            var cities = new[] { "London", "New York", "Shanghai", "Delhi", "Kabul" };
            foreach (var c in cities)
            {
                _database.Insert().Into("Cities")
                    .Set("Name", c)
                    .Run();
            }

            _database.Command("create edge Lives from (select from Persons where Name = 'Michael') to (select from Cities where Name = 'Kabul')");
            _database.Command("create edge Lives from (select from Persons where Name = 'Roman') to (select from Cities where Name = 'London')");
            _database.Command("create edge Lives from (select from Persons where Name = 'Luca') to (select from Cities where Name = 'New York')");
            _database.Command("create edge Lives from (select from Persons where Name = 'Nick') to (select from Cities where Name = 'Kabul')");
            _database.Command("create edge Lives from (select from Persons where Name = 'Fredi') to (select from Cities where Name = 'Delphi')");


            _database.Command("create edge Owns from (select from Persons where Name = 'Fredi') to (select from Cars where Name in ['BMW','Subaru'])");
            _database.Command("create edge Owns from (select from Persons where Name = 'Luca') to (select from Cars where Name in ['BMW','Subaru','Audi'])");
            _database.Command("create edge Owns from (select from Persons where Name = 'Nick') to (select from Cars where Name in ['Mazda'])");
            _database.Command("create edge Owns from (select from Persons where Name = 'Roman') to (select from Cars where Name in ['Subaru'])");


            // Query vertex
            var vertexes = _database.Query("select from Persons");

            foreach (var v in vertexes)
            {
                foreach (var kv in v)
                {
                    Debug.WriteLine(String.Format("{0}: {1} ({2})", kv.Key, kv.Value, kv.Value.GetType()));
                }
            }

            // This is output of a program, all properties have strong type and outgoin and incoming edges
            // have format [out | in]_EDGECLASS and type is List<ORID>
            /*
             
                @ORID: #13:0 (Orient.Client.ORID)
                @OVersion: 2 (System.Int32)
                @OType: Document (Orient.Client.ORecordType)
                @OClassId: 0 (System.Int16)
                @OClassName: Persons (System.String)
                Name: Michael (System.String)
                out_Lives: System.Collections.Generic.List`1[Orient.Client.ORID] (System.Collections.Generic.List`1[Orient.Client.ORID])
             
                @ORID: #13:1 (Orient.Client.ORID)
                @OVersion: 3 (System.Int32)
                @OType: Document (Orient.Client.ORecordType)
                @OClassId: 0 (System.Int16)
                @OClassName: Persons (System.String)
                Name: Roman (System.String)
                out_Lives: System.Collections.Generic.List`1[Orient.Client.ORID] (System.Collections.Generic.List`1[Orient.Client.ORID])
                out_Owns: System.Collections.Generic.List`1[Orient.Client.ORID] (System.Collections.Generic.List`1[Orient.Client.ORID])
                
                @ORID: #13:2 (Orient.Client.ORID)
                @OVersion: 5 (System.Int32)
                @OType: Document (Orient.Client.ORecordType)
                @OClassId: 0 (System.Int16)
                @OClassName: Persons (System.String)
                Name: Luca (System.String)
                out_Lives: System.Collections.Generic.List`1[Orient.Client.ORID] (System.Collections.Generic.List`1[Orient.Client.ORID])
                out_Owns: System.Collections.Generic.List`1[Orient.Client.ORID] (System.Collections.Generic.List`1[Orient.Client.ORID])
              
                @ORID: #13:3 (Orient.Client.ORID)
                @OVersion: 3 (System.Int32)
                @OType: Document (Orient.Client.ORecordType)
                @OClassId: 0 (System.Int16)
                @OClassName: Persons (System.String)
                Name: Nick (System.String)
                out_Lives: System.Collections.Generic.List`1[Orient.Client.ORID] (System.Collections.Generic.List`1[Orient.Client.ORID])
                out_Owns: System.Collections.Generic.List`1[Orient.Client.ORID] (System.Collections.Generic.List`1[Orient.Client.ORID])
             
                @ORID: #13:4 (Orient.Client.ORID)
                @OVersion: 3 (System.Int32)
                @OType: Document (Orient.Client.ORecordType)
                @OClassId: 0 (System.Int16)
                @OClassName: Persons (System.String)
                Name: Fredi (System.String)
                out_Owns: System.Collections.Generic.List`1[Orient.Client.ORID] (System.Collections.Generic.List`1[Orient.Client.ORID])
             
             */
        }
    }
}
