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
        static string _alias = "tinkerpop";

        static void Main(string[] args)
        {
            OClient.CreateDatabasePool(
                "127.0.0.1",
                //"vps-04-ubuntu-server.developmententity.sk",
                2424,
                //"tinkerpop",
                "test",
                ODatabaseType.Graph,
                "admin",
                "admin",
                10,
                _alias
            );

            //TestConnection();
            TestQuery();
            //TestLoad();

            System.Console.ReadLine();
        }

        static void TestConnection()
        {
            ODatabase database = new ODatabase(_alias);

            System.Console.WriteLine(database.GetClusters().Count);
        }

        static void TestQuery()
        {
            ODatabase database = new ODatabase(_alias);

            /*foreach (ORecord record in database.Query("select from OGraphVertex limit 20", "*:1"))
            {
                Ve v = record.To<Ve>();
                System.Console.WriteLine(v.Title);
            }

            /*foreach (ORecord record in database.Query("select from OGraphEdge limit 20", "*:2"))
            {
                Ed e = record.To<Ed>();
                System.Console.WriteLine(e.Label);
            }*/

            //object foo = database.Command("create vertex OGraphVertex set title = \"whoa\"");
            object foo = database.Command("delete vertex #8:8");
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
        }
    }

    class Vertex
    {
        [OProperty(MapTo = "name")]
        public string Name { get; set; }

        [OProperty(MapTo = "song_type")]
        public string SongType { get; set; }

        [OProperty(MapTo = "performances")]
        public int Performances { get; set; }

        [OProperty(MapTo = "type")]
        public string Type { get; set; }

        [OProperty(MapTo = "in")]
        public List<ORID> In { get; set; }

        [OProperty(MapTo = "out")]
        public List<ORID> Out { get; set; }
    }

    class Ve
    {
        [OProperty(MapTo = "title")]
        public string Title { get; set; }
    }

    class Ed
    {
        [OProperty(MapTo = "label")]
        public string Label { get; set; }
    }
}
