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
    public class GitHub_issue8
    {
        TestDatabaseContext _context;
        ODatabase _database;
        short _clusterId;

        [SetUp]
        public void Init()
        {
            _context = new TestDatabaseContext();
            _database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            _clusterId = _database.Create.Class("TestClass").Extends("V").Run();
        }

        [TearDown]
        public void Cleanup()
        {
            _database.Dispose();
            _context.Dispose();
        }

        [Test]
        public void ShouldExecuteQueryAfterClassIsChanged()
        {
            _database.Create.Vertex("TestClass")
                .Set("Name", "First Vertex")
                .Run();

            // Get clusters for class TestClass before changes

            var clustersBefore = _database.Schema.GetClustersForClass("TestClass");

            Assert.AreEqual(1, clustersBefore.Count());

            var result = _database.Command("ALTER CLASS TestClass Name Test");
            
            var cluster2id = _database.Create
                .Cluster("TestClassCluster2", OClusterType.Physical)
                .Run();

            _database.Command("alter class Test addcluster TestClassCluster2");

            var clustersAfter = _database.Schema.GetClustersForClass("Test");
            
            Assert.AreEqual(2, clustersAfter.Count());

            var doc = _database
                        .Create
                        .Vertex("Test")
                        .Set("Name", "Second Vertex")
                        .Run();

            var doc1 = _database
                        .Create
                        .Vertex("Test")
                        .Cluster("TestClassCluster2")
                        .Set("Name", "Third Vertex in same class but another cluster")
                        .Run();

            Assert.AreEqual(_clusterId, doc.ORID.ClusterId);
            Assert.AreEqual(cluster2id, doc1.ORID.ClusterId);
        }
    }
}
