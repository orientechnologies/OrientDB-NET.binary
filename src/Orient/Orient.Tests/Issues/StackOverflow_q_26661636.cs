using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Issues
{
    // http://stackoverflow.com/questions/26661636/orientdb-net-binary-for-models
    using q26661636;

    [TestClass]
    public class StackOverflow_q_26661636
    {
        TestDatabaseContext _context;
        ODatabase _database;

        [TestInitialize]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);

            _database.Create.Class<Person>().Extends<OVertex>().CreateProperties().Run();
            _database.Create.Class<Country>().Extends<OVertex>().CreateProperties().Run();
            _database.Create.Class<Car>().Extends<OVertex>().CreateProperties().Run();
            _database.Create.Class("Owns").Extends<OEdge>().Run();
            _database.Create.Class("Lives").Extends<OEdge>().Run();

        }

        [TestCleanup]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [TestMethod]
        [TestCategory("Stackoverflow")]
        public void q_26661636()
        {
            var lukaPerson = new Person { Name = "Luca" };
            var lpV = _database.Create.Vertex(lukaPerson).Run();

            var ferrariModenaCar = new Car { Name = "Ferrari Modena" };
            var fmcV = _database.Create.Vertex(ferrariModenaCar).Run();
            var bmwCar = new Car { Name = "BMW" };
            var bmwcV = _database.Create.Vertex(bmwCar).Run();
            var lp_fmcE = _database.Create.Edge("Owns").From(lpV.ORID).To(fmcV.ORID).Run();
            var lp_bmwcE = _database.Create.Edge("Owns").From(lpV.ORID).To(bmwcV.ORID).Run();

            var countryUS = new Country { Name = "US" };
            var uscV = _database.Create.Vertex(countryUS).Run();
            var lp_uscE = _database.Create.Edge("Lives").From(lpV.ORID).To(uscV.ORID).Run();

            var countryUK = new Country { Name = "UK" };
            var ukcV = _database.Create.Vertex(countryUK).Run();

            var pl = _database.Select().From<Person>().ToList<Person>().FirstOrDefault(p => p.Name == lukaPerson.Name);

            Assert.IsNotNull(pl);
            Assert.AreEqual(lukaPerson.Name, pl.Name);
            Assert.AreEqual(1, pl.out_Lives.Count);
            Assert.AreEqual(2, pl.out_Owns.Count);
        }
    }

    namespace q26661636
    {
        public class Person
        {
            public string Name { get; set; }
            public List<ORID> out_Lives { get; set; }
            public List<ORID> out_Owns { get; set; }
        }

        public class Country
        {
            public string Name { get; set; }
            public List<ORID> in_Lives { get; set; }
        }

        public class Car
        {
            public string Name { get; set; }
            public List<ORID> in_Owns { get; set; }
        }
    }
}
