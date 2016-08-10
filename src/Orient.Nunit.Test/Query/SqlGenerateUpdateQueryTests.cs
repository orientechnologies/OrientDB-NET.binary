using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlGenerateUpdateQueryTests
    {
        [Test]
        public void ShouldGenerateUpdateClassFromDocumentQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestVertexClass";
            document
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlUpdate()
                .Update(document)
                .ToString();

            string query =
                "UPDATE TestVertexClass " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateClassFromObjectQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.Name = "Johny";
            profile.Surname = "Bravo";

            string generatedQuery = new OSqlUpdate()
                .Update(profile)
                .ToString();

            string query =
                "UPDATE TestProfileClass " +
                "SET Name = 'Johny', " +
                "Surname = 'Bravo'";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateClassQuery()
        {
            ODocument document = new ODocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlUpdate()
                .Update(document)
                .Class("TestVertexClass")
                .ToString();

            string query =
                "UPDATE TestVertexClass " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateClusterQuery()
        {
            ODocument document = new ODocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlUpdate()
                .Update(document)
                .Cluster("TestCluster")
                .ToString();

            string query =
                "UPDATE cluster:TestCluster " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateRecordFromDocumentQuery()
        {
            ODocument document = new ODocument();
            document.ORID = new ORID(8, 0);
            document
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlUpdate()
                .Update(document)
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateRecordFromOridQuery()
        {
            ODocument document = new ODocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlUpdate()
                .Update(new ORID(8, 0))
                .Set(document)
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateRecordQuery()
        {
            ODocument document = new ODocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlUpdate()
                .Update(document)
                .Record(new ORID(8, 0))
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateRecordSetQuery()
        {
            string generatedQuery = new OSqlUpdate()
                .Record(new ORID(8, 0))
                .Set("foo", "foo string value")
                .Set("bar", 12345)
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateWhereLimitQuery()
        {
            ODocument document = new ODocument();
            document.ORID = new ORID(8, 0);
            document
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlUpdate()
                .Update(document)
                .Where("foo").Equals("whoa")
                .Or("foo").NotEquals(123)
                .And("foo").Lesser(1)
                .And("foo").LesserEqual(2)
                .And("foo").Greater(3)
                .And("foo").GreaterEqual(4)
                .And("foo").Like("%whoa%")
                .And("foo").IsNull()
                .And("foo").Contains("johny")
                .And("foo").Contains("name", "johny")
                .Limit(5)
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "SET foo = 'foo string value', " +
                "bar = 12345 " +
                "WHERE foo = 'whoa' " +
                "OR foo != 123 " +
                "AND foo < 1 " +
                "AND foo <= 2 " +
                "AND foo > 3 " +
                "AND foo >= 4 " +
                "AND foo LIKE '%whoa%' " +
                "AND foo IS NULL " +
                "AND foo CONTAINS 'johny' " +
                "AND foo CONTAINS (name = 'johny') " +
                "LIMIT 5";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateAddCollectionItemQuery()
        {
            string generatedQuery = new OSqlUpdate()
                .Record(new ORID(8, 0))
                .Add("foo", "foo string value")
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "ADD foo = 'foo string value'";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateRemoveCollectionItemQuery()
        {
            string generatedQuery = new OSqlUpdate()
                .Record(new ORID(8, 0))
                .Remove("foo", 123)
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "REMOVE foo = 123";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateUpdateRemoveFieldsQuery()
        {
            string generatedQuery = new OSqlUpdate()
                .Record(new ORID(8, 0))
                .Remove("foo")
                .Remove("bar")
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "REMOVE foo, bar";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
