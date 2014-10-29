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
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database.Create.Class<UnorderedWidget>().Extends("V").CreateProperties().Run();
                    database.Create.Class<WidgetVersion>().Extends("V").CreateProperties().Run();

                    database.Create.Class("VersionOf").Extends("E").Run();
                    database.Command(string.Format("create property {0}.out link {1}", "VersionOf", typeof(WidgetVersion).Name));
                    database.Command(string.Format("create property {0}.in link {1}", "VersionOf", typeof(UnorderedWidget).Name));


                    var w1 = new UnorderedWidget() { Value = 12 };
                    var wv1 = new WidgetVersion() { Value = 23 };
                    var wv2 = new WidgetVersion() { Value = 34 };

                    var v1 = database.Create.Vertex(w1).Run();
                    var v2 = database.Create.Vertex(wv1).Run();
                    var v3 = database.Create.Vertex(wv2).Run();

                    var e1 = database.Create.Edge("VersionOf").From(v1).To(v2).Run();
                    var e2 = database.Create.Edge("VersionOf").From(v1).To(v3).Run();

                    var loaded1 = database.Load.ORID(v1.ORID).Run();
                    var typed1 = loaded1.To<UnorderedWidget>();
                    Assert.AreEqual(2, typed1.out_VersionOf.Count);
                    Assert.AreEqual(12, typed1.Value);

                    var loaded2 = database.Load.ORID(v2.ORID).Run();
                    var typed2 = loaded2.To<WidgetVersion>();
                    Assert.IsNotNull(typed2.in_VersionOf);
                    var x = database.Command("select from E");
                }
            }
        }

        [TestMethod]
        public void TestOrderedEdges()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database.Create.Class<Widget>().Extends("V").CreateProperties().Run();
                    database.Create.Class<WidgetVersion>().Extends("V").CreateProperties().Run();

                    database.Create.Class("VersionOf").Extends("E").Run();
                    database.Command(string.Format("create property {0}.out link {1}", "VersionOf", typeof(WidgetVersion).Name));
                    database.Command(string.Format("create property {0}.in link {1}", "VersionOf", typeof(Widget).Name));

                    database.Command(string.Format("create property {1}.in_{0} ANY", "VersionOf", typeof(Widget).Name));
                    database.Command(string.Format("alter property {1}.in_{0} custom ordered=true", "VersionOf", typeof(Widget).Name));

                    var w1 = new Widget() { Value = 12 };
                    var wv1 = new WidgetVersion() { Value = 23 };
                    var wv2 = new WidgetVersion() { Value = 34 };

                    var v1 = database.Create.Vertex(w1).Run();
                    var v2 = database.Create.Vertex(wv1).Run();
                    var v3 = database.Create.Vertex(wv2).Run();

                    database.Create.Edge("VersionOf").From(v1).To(v2).Run();
                    database.Create.Edge("VersionOf").From(v1).To(v3).Run();

                    var loaded1 = database.Load.ORID(v1.ORID).Run();
                    var typed1 = loaded1.To<Widget>();
                    Assert.AreEqual(2, typed1.out_VersionOf.Count);
                    Assert.AreEqual(12, typed1.Value);

                    var loaded2 = database.Load.ORID(v2.ORID).Run();
                    var typed2 = loaded2.To<WidgetVersion>();
                    Assert.IsNotNull(typed2.in_VersionOf);

                }
            }
        }
    }
}
