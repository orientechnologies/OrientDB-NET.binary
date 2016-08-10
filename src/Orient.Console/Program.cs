using Orient.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Orient.Console
{

    public class Program
    {
        public static IConfiguration Configuration { get; set; }


        public static void Main()
        {

            Configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();



            ConnectionPoolTest();
            //CreateDatabaseTestManualy();
            //    System.Console.WriteLine("Press any key to exit ...");
            //    System.Console.ReadKey(true);
        }

        private static void ConnectionPoolTest()
        {
            OServer server = new OServer(Configuration["ConnectionStrings:DefaultConnection:Server"],
                int.Parse(Configuration["ConnectionStrings:DefaultConnection:Port"]),
                Configuration["ConnectionStrings:DefaultConnection:Username"],
                Configuration["ConnectionStrings:DefaultConnection:Password"]);

            if (server.DatabaseExist(Configuration["ConnectionStrings:DefaultConnection:DefaultDB"], OStorageType.PLocal))
            {
                server.DropDatabase(Configuration["ConnectionStrings:DefaultConnection:DefaultDB"], OStorageType.PLocal);
            }

            server.CreateDatabase(Configuration["ConnectionStrings:DefaultConnection:DefaultDB"], ODatabaseType.Graph, OStorageType.PLocal);

            using (ODatabase database = new ODatabase(Configuration["ConnectionStrings:DefaultConnection:Server"],
                int.Parse(Configuration["ConnectionStrings:DefaultConnection:Port"]),
                Configuration["ConnectionStrings:DefaultConnection:DefaultDB"],
                ODatabaseType.Graph,
                Configuration["ConnectionStrings:DefaultConnection:Username"],
                Configuration["ConnectionStrings:DefaultConnection:Password"]))
            {
               
                database.Create.Class("Person").Run();
                database.Insert().Into("Person").Set("Name", $"John").Set("LastName", $"Doe").Set("Age", 99).Run();
                Parallel.For(0, 1000000, (i) =>
                {
                    ODocument document = database.Insert().Into("Person").Set("Name", $"John{i}").Set("LastName", $"Doe{i}").Set("Age", i).Run();
                    //System.Console.WriteLine(document.ORID);
                });
            }
        }

        //static void CreateDatabaseTestManualy()
        //{
        //var serverConnInfo = Configuration.GetSection("ConnectionStrings:DefaultConnection").GetChildren().ToDictionary(x=>x.Key);
        //using (var server = new OServer(serverConnInfo["Server"].Value, int.Parse(serverConnInfo["Port"].Value), serverConnInfo["Username"].Value, serverConnInfo["Password"].Value))
        //    {
        //        var created = false;
        //        try
        //        {
        //            var exists = server.DatabaseExist("TestManualy", OStorageType.PLocal);
        //            if (exists) server.DropDatabase("TestManualy", OStorageType.PLocal);

        //            created = server.CreateDatabase("TestManualy", ODatabaseType.Document, OStorageType.PLocal);

        //            if (!created)
        //                throw new Exception("Database not created");

        //            exists = server.DatabaseExist("TestManualy", OStorageType.PLocal);

        //            if (!exists)
        //                throw new Exception("Database not exists");

        //            System.Console.WriteLine("Database created - get server configuration");

        //            var config = server.ConfigList();
        //            foreach (var item in config)
        //            {
        //                System.Console.WriteLine("{0} : {1}",
        //                    item.Key, item.Value);
        //            }

        //            System.Console.WriteLine("try connect to the database and query");

        //            OClient.CreateDatabasePool(
        //                    serverConnInfo["Server"].Value,
        //                    int.Parse(serverConnInfo["Port"].Value),
        //                    "TestManualy",
        //                    ODatabaseType.Graph,
        //                    serverConnInfo["Username"].Value,
        //                    serverConnInfo["Password"].Value,
        //                    10,
        //                    "AppConnection"
        //                );
        //            using (var database = new ODatabase("AppConnection"))
        //            {
        //                var documents = database.Query("select from OUser");
        //                foreach (var item in documents)
        //                {
        //                    System.Console.WriteLine("Name: {0} Status: {1}",
        //                        item.GetField<string>("name"), item.GetField<string>("status"));
        //                }
        //            }
        //            OClient.DropDatabasePool("AppConnection");
        //        }
        //        finally
        //        {
        //            if (created)
        //                server.DropDatabase("TestManualy", OStorageType.PLocal);
        //        }
        //    }
        //}

        //static void CreateDatabaseTestUsingContext()
        //{
        //    using (var context = new TestDatabaseContext())
        //    using (var database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
        //    {
        //        var documents = database.Query("select from OUser");
        //        foreach (var item in documents)
        //        {
        //            System.Console.WriteLine("Name: {0} Status: {1}",
        //                item.GetField<string>("name"), item.GetField<string>("status"));
        //        }
        //    }
        //}

        //static void CreateDatabasePoolTest()
        //{
        //    var serverConnInfo = Configuration.GetSection("ConnectionStrings:DefaultConnection").GetChildren().ToDictionary(x => x.Key);

        //    OClient.CreateDatabasePool(
        //                serverConnInfo["Server"].Value,
        //                int.Parse(serverConnInfo["Port"].Value),
        //                serverConnInfo["DefaultDB"].Value,
        //                ODatabaseType.Graph,
        //               serverConnInfo["Username"].Value,
        //                serverConnInfo["Password"].Value,
        //                10,
        //                "AppConnection"
        //            );

        //    using (var database = new ODatabase("AppConnection"))
        //    {
        //        var documents = database.Query("select from v");
        //        foreach (var item in documents)
        //        {
        //            System.Console.WriteLine("Name: {0} Type: {1}",
        //                item.GetField<string>("name"), item.GetField<string>("type"));
        //        }
        //    }

        //    OClient.DropDatabasePool("AppConnection");
        //}

        //    static void ConnectionPoolTest()
        //    {
        //        using (TestDatabaseContext testContext = new TestDatabaseContext())
        //        {
        //            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
        //            {
        //                database
        //                    .Create.Class("Person")
        //                    .Extends<OVertex>()
        //                    .Run();

        //                database
        //                    .Create.Class("Spouse")
        //                    .Extends<OVertex>()
        //                    .Run();

        //                ODocument person1 = database
        //                    .Create.Vertex("Person")
        //                    .Set("Name", "Johny")
        //                    .Run();

        //                ODocument spouse1 = database
        //                    .Create.Vertex("Spouse")
        //                    .Set("Name", "Mary")
        //                    .Run();

        //                ODocument spouse2 = database
        //                    .Create.Vertex("Spouse")
        //                    .Set("Name", "Julia")
        //                    .Run();

        //                // TODO: check what happens in command execution
        //                ODocument edge1 = database
        //                    .Create.Edge<OEdge>()
        //                    .From(person1)
        //                    .To(spouse1)
        //                    .Run();

        //                ODocument edge2 = database
        //                    .Create.Edge<OEdge>()
        //                    .From(person1)
        //                    .To(spouse2)
        //                    .Run();

        //                List<ODocument> docs = database.Query("select from Person");
        //            }

        //            bool exit = false;

        //            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
        //            {
        //                database
        //                    .Create.Class("TestClass")
        //                    .Extends<OVertex>()
        //                    .Run();

        //                database
        //                    .Create.Vertex("TestClass")
        //                    .Set("foo", "foo string value 1")
        //                    .Set("bar", 123)
        //                    .Run();

        //                database
        //                    .Create.Vertex("TestClass")
        //                    .Set("foo", "foo string value 2")
        //                    .Set("bar", 1233)
        //                    .Run();
        //            }

        //            while (!exit)
        //            {
        //                System.Console.WriteLine(
        //                    "Current pool size: {0} @ {1} : {2}",
        //                    OClient.DatabasePoolCurrentSize(TestConnection.GlobalTestDatabaseAlias),
        //                    DateTime.Now.ToString(),
        //                    Query().Count
        //                );

        //                string line = System.Console.ReadLine();

        //                if (line.Equals("exit"))
        //                {
        //                    exit = true;
        //                }
        //            }
        //        }
        //    }
        //    static List<ODocument> Query()
        //    {
        //        List<ODocument> documents;

        //        try
        //        {
        //            using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
        //            {
        //                documents = database
        //                    .Select()
        //                    .From("TestClass")
        //                    .ToList();
        //                using (ODatabase database1 = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
        //                {
        //                    List<ODocument> documents2 = database1
        //                        .Select()
        //                        .From("TestClass")
        //                        .ToList();
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {
        //            System.Console.WriteLine(e.Message);
        //            documents = new List<ODocument>();
        //        }

        //        return documents;
        //    }
        //}

        //class Vertex
        //{
        //    [OProperty(Alias = "name")]
        //    public string Name { get; set; }

        //    [OProperty(Alias = "song_type")]
        //    public string SongType { get; set; }

        //    [OProperty(Alias = "performances")]
        //    public int Performances { get; set; }

        //    [OProperty(Alias = "type")]
        //    public string Type { get; set; }

        //    [OProperty(Alias = "in")]
        //    public List<ORID> In { get; set; }

        //    [OProperty(Alias = "out")]
        //    public List<ORID> Out { get; set; }
        //}

        //class Ve
        //{
        //    [OProperty(Alias = "title")]
        //    public string Title { get; set; }
        //}

        //class Ed
        //{
        //    [OProperty(Alias = "label")]
        //    public string Label { get; set; }
        //}
    }
}
