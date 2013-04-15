using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orient.Client;

namespace Orient.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                bool exit = false;


                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class("TestClass")
                        .Extends<OGraphVertex>()
                        .Run();

                    database
                        .Create.Vertex("TestClass")
                        .Set("foo", "foo string value 1")
                        .Set("bar", 123)
                        .Run();

                    database
                        .Create.Vertex("TestClass")
                        .Set("foo", "foo string value 2")
                        .Set("bar", 1233)
                        .Run();
                }

                while (!exit)
                {
                    System.Console.WriteLine(
                        "Current pool size: {0} @ {1} : {2}", 
                        OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias), 
                        DateTime.Now.ToString(),
                        Query().Count
                    );

                    string line = System.Console.ReadLine();

                    if (line.Equals("exit"))
                    {
                        exit = true;
                    }
                }
            }
        }

        static List<ORecord> Query()
        {
            List<ORecord> records;

            try
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    records = database
                        .Select()
                        .From("TestClass")
                        .ToList();
                    using (ODatabase database1 = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                    {
                        List<ORecord> records2 = database1
                            .Select()
                            .From("TestClass")
                            .ToList();
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                records = new List<ORecord>();
            }

            return records;
        }

        /*static void TestConnection()
        {
            ODatabase database = new ODatabase(_alias);

            System.Console.WriteLine(database.GetClusters().Count);
        }

        static void TestQuery()
        {
            ODatabase database = new ODatabase(_alias);

            //List<ORecord> records = database.Query("select from OGraphVertex where title = \"server 1\"", "*:2");
            //List<ORecord> records = database.Query("select in.in.@rid as inVs, in.out.@rid as outVs, title from OGraphVertex where @rid = #8:0");
            List<ORecord> records = database.Query(
                "select " +
                " out.in as neighborRIDs," +
                //" out.in.type as neighborTypes," +
                //" out.in.state as neighborStates," +
                " title," +
                " @rid" +
                " from #8:0"
            );

            foreach (ORecord record in records)
            {
                //Ve v = record.To<Ve>();
                List<ORID> orids = record.GetField<List<ORID>>("neighborRIDs");
                List<string> types = record.GetField<List<string>>("neighborTypes");
                List<int> states = record.GetField<List<int>>("neighborStates");

                System.Console.WriteLine("{0} - {1} {2} {3}", record.GetField<string>("title"), orids.Count, types.Count, states.Count);
                //System.Console.WriteLine(v.Title);
            }

            //foreach (ORecord record in database.Query("select from OGraphEdge limit 20", "*:2"))
            //{
            //    Ed e = record.To<Ed>();
            //    System.Console.WriteLine(e.Label);
            //}

            //ORecord rec = database.Command("create vertex OGraphVertex set title = \"whoa\"").ToSingle();
            //object foo = database.Command("delete vertex " + rec.ORID.ToString());
        }

        static void TestLoad()
        {
            int runs = 20;
            long total = 0;

            for (int i = 0; i < runs; i++)
            {
                long tps = Do();
                total += tps;

                System.Console.WriteLine("TPS: " + tps);
            }

            System.Console.WriteLine("Average: " + total / runs);
        }

        static long Do()
        {
            DateTime start = DateTime.Now;
            bool running = true;
            long tps = 0;

            do
            {
                ODatabase database = new ODatabase(_alias);

                //List<ORecord> result = database.Query("select name from OGraphVertex where in[0].label = 'followed_by' and in[0].out.name = 'JAM'");
                //List<ORecord> result = database.Query("select from OGraphVertex limit 20");
                List<ORecord> result = database.Query("select from OGraphEdge limit 20");

                database.Close();
                tps++;

                TimeSpan dif = DateTime.Now - start;

                if (dif.TotalMilliseconds > 1000)
                {
                    running = false;
                }
            }
            while (running);

            return tps;
        }*/
    }

    class Vertex
    {
        [OProperty(MappedTo = "name")]
        public string Name { get; set; }

        [OProperty(MappedTo = "song_type")]
        public string SongType { get; set; }

        [OProperty(MappedTo = "performances")]
        public int Performances { get; set; }

        [OProperty(MappedTo = "type")]
        public string Type { get; set; }

        [OProperty(MappedTo = "in")]
        public List<ORID> In { get; set; }

        [OProperty(MappedTo = "out")]
        public List<ORID> Out { get; set; }
    }

    class Ve
    {
        [OProperty(MappedTo = "title")]
        public string Title { get; set; }
    }

    class Ed
    {
        [OProperty(MappedTo = "label")]
        public string Label { get; set; }
    }
}
