using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Orient.Client;

namespace Orient.Tests.Issues
{
    [TestFixture]
    public class GitHub_issue2
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
        public void CreateVertexWithHeavyEdgeTX()
        {
            var v1 = new ODocument { OClassName = "TestVertex" };
            v1.SetField("Name", "First");
            v1.SetField("Bar", 1);

            var v2 = new ODocument { OClassName = "TestVertex" };
            v2.SetField("Name", "Second");
            v2.SetField("Bar", 2);

            var e1 = new ODocument { OClassName = "TestEdge" };
            e1.SetField("Weight", 1.3f);

            // Add records to the transaction
            _database.Transaction.Add(v1);
            _database.Transaction.Add(v2);
            _database.Transaction.Add(e1);

            // link records
            v1.SetField("in_TestEdge", e1.ORID);
            v2.SetField("out_TestEdge", e1.ORID);
            e1.SetField("in", v1.ORID);
            e1.SetField("out", v2.ORID);

            _database.Transaction.Commit();

            Assert.IsNotNull(v1.ORID);
            Assert.IsNotNull(v2.ORID);
            Assert.IsNotNull(e1.ORID);

            var lv1 = _database.Load.ORID(v1.ORID).Run();
            var lv2 = _database.Load.ORID(v2.ORID).Run();
            var le1 = _database.Load.ORID(e1.ORID).Run();

            Assert.AreEqual(v1.GetField<string>("Name"), lv1.GetField<string>("Name"));
            Assert.AreEqual(v1.GetField<int>("Bar"), lv1.GetField<int>("Bar"));

            Assert.AreEqual(v2.GetField<string>("Name"), lv2.GetField<string>("Name"));
            Assert.AreEqual(v2.GetField<int>("Bar"), lv2.GetField<int>("Bar"));

            Assert.AreEqual(e1.GetField<float>("Weight"), le1.GetField<float>("Weight"));

            Assert.AreEqual(e1.ORID, lv1.GetField<ORID>("in_TestEdge"));
            Assert.AreEqual(e1.ORID, lv2.GetField<ORID>("out_TestEdge"));

            Assert.AreEqual(v1.ORID, le1.GetField<ORID>("in"));
            Assert.AreEqual(v2.ORID, le1.GetField<ORID>("out"));

        }
    }
}
