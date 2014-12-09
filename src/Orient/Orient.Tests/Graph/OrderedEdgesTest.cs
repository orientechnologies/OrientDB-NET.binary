using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Graph
{
    [TestClass]
    public class OrderedEdgesTests
    {
        class Widget
        {
            public int Value { get; set; }
            public List<ORID> out_VersionOf { get; set; }
        }

        class UnorderedWidget
        {
            public int Value { get; set; }
            public HashSet<ORID> out_VersionOf { get; set; }
        }

        class WidgetVersion
        {
            public int Value { get; set; }
            public HashSet<ORID> in_VersionOf { get; set; }

        }

        [TestMethod]
        public void TestUnOrderedEdges()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Create.Class<UnorderedWidget>().Extends("V").CreateProperties().Run();
                database.Create.Class<WidgetVersion>().Extends("V").CreateProperties().Run();

                database.Create.Class("VersionOf").Extends("E").Run();

                var w1 = new UnorderedWidget() { Value = 12 };
                var wv1 = new WidgetVersion() { Value = 23 };
                var wv2 = new WidgetVersion() { Value = 34 };

                var v1 = database.Create.Vertex(w1).Run();
                var v2 = database.Create.Vertex(wv1).Run();
                var v3 = database.Create.Vertex(wv2).Run();

                var e1 = database.Create.Edge("VersionOf").From(v1).To(v2).Run();
                var e2 = database.Create.Edge("VersionOf").From(v1).To(v3).Run();

                var loaded1 = database.Load.ORID(v1.ORID).Run<UnorderedWidget>();
                Assert.AreEqual(2, loaded1.out_VersionOf.Count);
                Assert.IsTrue(loaded1.out_VersionOf.Contains(v2.ORID));
                Assert.IsTrue(loaded1.out_VersionOf.Contains(v3.ORID));
                Assert.AreEqual(12, loaded1.Value);

                var loaded2 = database.Load.ORID(v2.ORID).Run<WidgetVersion>();
                Assert.IsNotNull(loaded2.in_VersionOf);
                Assert.AreEqual(1, loaded2.in_VersionOf.Count);
                Assert.AreEqual(23, loaded2.Value);
                Assert.IsTrue(loaded2.in_VersionOf.Contains(v1.ORID));

                var loaded3 = database.Load.ORID(v3.ORID).Run<WidgetVersion>();
                Assert.IsNotNull(loaded3.in_VersionOf);
                Assert.AreEqual(1, loaded3.in_VersionOf.Count);
                Assert.AreEqual(34, loaded3.Value);
                Assert.IsTrue(loaded3.in_VersionOf.Contains(v1.ORID));

            }
        }

        [TestMethod]
        public void TestOrderedEdges()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Create.Class<Widget>().Extends("V").CreateProperties().Run();
                database.Create.Class<WidgetVersion>().Extends("V").CreateProperties().Run();

                database.Create.Class("VersionOf").Extends("E").Run();

                var w1 = new Widget() { Value = 12 };
                var wv1 = new WidgetVersion() { Value = 23 };
                var wv2 = new WidgetVersion() { Value = 34 };

                var v1 = database.Create.Vertex(w1).Run();
                var v2 = database.Create.Vertex(wv1).Run();
                var v3 = database.Create.Vertex(wv2).Run();

                database.Create.Edge("VersionOf").From(v1).To(v2).Run();
                database.Create.Edge("VersionOf").From(v1).To(v3).Run();

                var loaded1 = database.Load.ORID(v1.ORID).Run<Widget>();
                Assert.AreEqual(2, loaded1.out_VersionOf.Count);
                Assert.IsTrue(loaded1.out_VersionOf.Contains(v2.ORID));
                Assert.IsTrue(loaded1.out_VersionOf.Contains(v3.ORID));
                Assert.AreEqual(12, loaded1.Value);

                var loaded2 = database.Load.ORID(v2.ORID).Run<WidgetVersion>();
                Assert.IsNotNull(loaded2.in_VersionOf);
                Assert.AreEqual(1, loaded2.in_VersionOf.Count);
                Assert.AreEqual(23, loaded2.Value);
                Assert.IsTrue(loaded2.in_VersionOf.Contains(v1.ORID));

                var loaded3 = database.Load.ORID(v3.ORID).Run<WidgetVersion>();
                Assert.IsNotNull(loaded3.in_VersionOf);
                Assert.AreEqual(1, loaded3.in_VersionOf.Count);
                Assert.AreEqual(34, loaded3.Value);
                Assert.IsTrue(loaded3.in_VersionOf.Contains(v1.ORID));

            }
        }
    }
}
