using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Graph
{
    
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

        [Fact]
        public void TestUnOrderedEdges()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Command("alter database custom useLightweightEdges=false");

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
                Assert.Equal(2, loaded1.out_VersionOf.Count);
                Assert.True(loaded1.out_VersionOf.Contains(e1.ORID));
                Assert.True(loaded1.out_VersionOf.Contains(e1.ORID));
                Assert.Equal(12, loaded1.Value);

                var loaded2 = database.Load.ORID(v2.ORID).Run<WidgetVersion>();
                Assert.NotNull(loaded2.in_VersionOf);
                Assert.Equal(1, loaded2.in_VersionOf.Count);
                Assert.Equal(23, loaded2.Value);
                Assert.True(loaded2.in_VersionOf.Contains(e1.ORID));

                var loaded3 = database.Load.ORID(v3.ORID).Run<WidgetVersion>();
                Assert.NotNull(loaded3.in_VersionOf);
                Assert.Equal(1, loaded3.in_VersionOf.Count);
                Assert.Equal(34, loaded3.Value);
                Assert.True(loaded3.in_VersionOf.Contains(e2.ORID));

            }
        }

        [Fact]
        public void TestOrderedEdges()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                database.Command("alter database custom useLightweightEdges=false");

                database.Create.Class<Widget>().Extends("V").CreateProperties().Run();
                database.Create.Class<WidgetVersion>().Extends("V").CreateProperties().Run();

                database.Create.Class("VersionOf").Extends("E").Run();

                var w1 = new Widget() { Value = 12 };
                var wv1 = new WidgetVersion() { Value = 23 };
                var wv2 = new WidgetVersion() { Value = 34 };

                var v1 = database.Create.Vertex(w1).Run();
                var v2 = database.Create.Vertex(wv1).Run();
                var v3 = database.Create.Vertex(wv2).Run();

                var e1 = database.Create.Edge("VersionOf").From(v1).To(v2).Run();
                var e2 = database.Create.Edge("VersionOf").From(v1).To(v3).Run();

                var loaded1 = database.Load.ORID(v1.ORID).Run<Widget>();
                Assert.Equal(2, loaded1.out_VersionOf.Count);
                Assert.True(loaded1.out_VersionOf.Contains(e1.ORID));
                Assert.True(loaded1.out_VersionOf.Contains(e2.ORID));
                Assert.Equal(12, loaded1.Value);

                var loaded2 = database.Load.ORID(v2.ORID).Run<WidgetVersion>();
                Assert.NotNull(loaded2.in_VersionOf);
                Assert.Equal(1, loaded2.in_VersionOf.Count);
                Assert.Equal(23, loaded2.Value);
                Assert.True(loaded2.in_VersionOf.Contains(e1.ORID));

                var loaded3 = database.Load.ORID(v3.ORID).Run<WidgetVersion>();
                Assert.NotNull(loaded3.in_VersionOf);
                Assert.Equal(1, loaded3.in_VersionOf.Count);
                Assert.Equal(34, loaded3.Value);
                Assert.True(loaded3.in_VersionOf.Contains(e2.ORID));

            }
        }
    }
}
