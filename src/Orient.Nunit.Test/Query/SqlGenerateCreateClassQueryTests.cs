using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlGenerateCreateClassQueryTests
    {
        [Test]
        public void ShouldGenerateCreateClassQuery()
        {
            string generatedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateClassExtendsQuery()
        {
            string generatedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .Extends("TestSuperClass")
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass " +
                "EXTENDS TestSuperClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateCreateClassExtendsClusterQuery()
        {
            string generatedQuery = new OSqlCreateClass()
                .Class("TestVertexClass")
                .Extends("TestSuperClass")
                .Cluster(8)
                .ToString();

            string query =
                "CREATE CLASS TestVertexClass " +
                "EXTENDS TestSuperClass " +
                "CLUSTER 8";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
