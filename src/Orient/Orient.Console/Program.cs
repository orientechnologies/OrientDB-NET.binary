﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orient.Client;

namespace Orient.Console
{
    class Program
    {
        public void Main(string[] args)
        {
            CreateDatabaseTestManualy();
            System.Console.WriteLine("Press any key to exit ...");
            System.Console.ReadKey(true);
        }

        static void CreateDatabaseTestManualy()
        {
            using (var server = new OServer("127.0.0.1", 2424, "root", "root"))
            {
                var created = false;
                try
                {
                    created = server.CreateDatabase("TestManualy", ODatabaseType.Document, OStorageType.PLocal);

                    if (!created)
                        throw new Exception("Database not created");

                    var exists = server.DatabaseExist("TestManualy", OStorageType.PLocal);

                    if (!exists)
                        throw new Exception("Database not exists");

                    System.Console.WriteLine("Database created - get server configuration");

                    var config = server.ConfigList();
                    foreach (var item in config)
                    {
                        System.Console.WriteLine("{0} : {1}",
                            item.Key, item.Value);
                    }

                    System.Console.WriteLine("try connect to the database and query");

                    OClient.CreateDatabasePool(
                            "localhost",
                            2424,
                            "TestManualy",
                            ODatabaseType.Graph,
                            "root",
                            "root",
                            10,
                            "AppConnection"
                        );
                    using (var database = new ODatabase("AppConnection"))
                    {
                        var documents = database.Query("select from OUser");
                        foreach (var item in documents)
                        {
                            System.Console.WriteLine("Name: {0} Status: {1}",
                                item.GetField<string>("name"), item.GetField<string>("status"));
                        }
                    }
                    OClient.DropDatabasePool("AppConnection");
                }
                finally
                {
                    if (created)
                        server.DropDatabase("TestManualy", OStorageType.PLocal);
                }
            }
        }

        static void CreateDatabaseTestUsingContext()
        {
            using (var context = new TestDatabaseContext())
            using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
            {
                var documents = database.Query("select from OUser");
                foreach (var item in documents)
                {
                    System.Console.WriteLine("Name: {0} Status: {1}",
                        item.GetField<string>("name"), item.GetField<string>("status"));
                }
            }
        }
        static void CreateDatabasePoolTest()
        {
            OClient.CreateDatabasePool(
                    "localhost",
                    2424,
                    "GratefulDeadConcerts",
                    ODatabaseType.Graph,
                    "root",
                    "root",
                    10,
                    "AppConnection"
                );

            using (var database = new ODatabase("AppConnection"))
            {
                var documents = database.Query("select from v");
                foreach (var item in documents)
                {
                    System.Console.WriteLine("Name: {0} Type: {1}",
                        item.GetField<string>("name"), item.GetField<string>("type"));
                }
            }

            OClient.DropDatabasePool("AppConnection");
        }
        static void ConnectionPoolTest()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class("Person")
                        .Extends<OVertex>()
                        .Run();

                    database
                        .Create.Class("Spouse")
                        .Extends<OVertex>()
                        .Run();

                    ODocument person1 = database
                        .Create.Vertex("Person")
                        .Set("Name", "Johny")
                        .Run();

                    ODocument spouse1 = database
                        .Create.Vertex("Spouse")
                        .Set("Name", "Mary")
                        .Run();

                    ODocument spouse2 = database
                        .Create.Vertex("Spouse")
                        .Set("Name", "Julia")
                        .Run();

                    // TODO: check what happens in command execution
                    ODocument edge1 = database
                        .Create.Edge<OEdge>()
                        .From(person1)
                        .To(spouse1)
                        .Run();

                    ODocument edge2 = database
                        .Create.Edge<OEdge>()
                        .From(person1)
                        .To(spouse2)
                        .Run();

                    List<ODocument> docs = database.Query("select from Person");
                }

                bool exit = false;

                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    database
                        .Create.Class("TestClass")
                        .Extends<OVertex>()
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
        static List<ODocument> Query()
        {
            List<ODocument> documents;

            try
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    documents = database
                        .Select()
                        .From("TestClass")
                        .ToList();
                    using (ODatabase database1 = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                    {
                        List<ODocument> documents2 = database1
                            .Select()
                            .From("TestClass")
                            .ToList();
                    }
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
                documents = new List<ODocument>();
            }

            return documents;
        }

        /*static void TestConnection()
        {
            ODatabase database = new ODatabase(_alias);

            System.Console.WriteLine(database.GetClusters().Count);
        }

        static void TestQuery()
        {
            ODatabase database = new ODatabase(_alias);

            //List<ORecord> records = database.Query("select from OVertex where title = \"server 1\"", "*:2");
            //List<ORecord> records = database.Query("select in.in.@rid as inVs, in.out.@rid as outVs, title from OVertex where @rid = #8:0");
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

            //foreach (ORecord record in database.Query("select from OEdge limit 20", "*:2"))
            //{
            //    Ed e = record.To<Ed>();
            //    System.Console.WriteLine(e.Label);
            //}

            //ORecord rec = database.Command("create vertex OVertex set title = \"whoa\"").ToSingle();
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

                //List<ORecord> result = database.Query("select name from OVertex where in[0].label = 'followed_by' and in[0].out.name = 'JAM'");
                //List<ORecord> result = database.Query("select from OVertex limit 20");
                List<ORecord> result = database.Query("select from OEdge limit 20");

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
        [OProperty(Alias = "name")]
        public string Name { get; set; }

        [OProperty(Alias = "song_type")]
        public string SongType { get; set; }

        [OProperty(Alias = "performances")]
        public int Performances { get; set; }

        [OProperty(Alias = "type")]
        public string Type { get; set; }

        [OProperty(Alias = "in")]
        public List<ORID> In { get; set; }

        [OProperty(Alias = "out")]
        public List<ORID> Out { get; set; }
    }

    class Ve
    {
        [OProperty(Alias = "title")]
        public string Title { get; set; }
    }

    class Ed
    {
        [OProperty(Alias = "label")]
        public string Label { get; set; }
    }
}
