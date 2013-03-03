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
                //"127.0.0.1",
                "vps-04-ubuntu-server.developmententity.sk",
                2424,
                "tinkerpop",
                ODatabaseType.Graph,
                "admin",
                "admin",
                10,
                _alias
            );

            //TestConnection();
            //TestQuery();
            TestLoad();

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

            foreach (string s in database.Command("select from OGraphEdge limit 20"))
            {
                System.Console.WriteLine(s);
            }
            //System.Console.WriteLine(database.Command("select from OGraphVertex limit 1").Count);
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

                //List<string> result = database.Command("select name from OGraphVertex where in[0].label = 'followed_by' and in[0].out.name = 'JAM'");
                //List<string> result = database.Command("select from OGraphVertex limit 20");
                List<string> result = database.Command("select from OGraphEdge limit 20");

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
}
