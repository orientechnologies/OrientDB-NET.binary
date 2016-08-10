using NUnit.Framework;
using Orient.Client;

namespace Orient.Nunit.Test.Query
{
   [TestFixture]
    public class SqlGenerateDeleteEdgeQueryTests
    {
        [Test]
        public void ShouldGenerateDeleteEdgeFromDocumentOridQuery()
        {
            ODocument document = new ODocument();
            document.ORID = new ORID(8, 0);

            string generatedQuery = new OSqlDeleteEdge()
                .Delete(document)
                .ToString();

            string query =
                "DELETE EDGE #8:0";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateDeleteEdgeFromObjectOridQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.ORID = new ORID(8, 0);

            string generatedQuery = new OSqlDeleteEdge()
                .Delete(profile)
                .ToString();

            string query =
                "DELETE EDGE #8:0";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateDeleteEdgeFromDocumentOClassNameQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestClass";

            string generatedQuery = new OSqlDeleteEdge()
                .Delete(document)
                .ToString();

            string query =
                "DELETE EDGE TestClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateDeleteEdgeFromObjectOClassNameQuery()
        {
            TestProfileClass profile = new TestProfileClass();

            string generatedQuery = new OSqlDeleteEdge()
                .Delete(profile)
                .ToString();

            string query =
                "DELETE EDGE TestProfileClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateDeleteEdgeClassWhereLimitQuery()
        {
            string generatedQuery = new OSqlDeleteEdge()
                .Class<TestProfileClass>()
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
                "DELETE EDGE TestProfileClass " +
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
        public void ShouldGenerateDeleteEdgeFromDocumentToDocumentQuery()
        {
            TestProfileClass profile = new TestProfileClass();

            ODocument vertex1 = new ODocument();
            vertex1.ORID = new ORID(8, 0);

            ODocument vertex2 = new ODocument();
            vertex2.ORID = new ORID(8, 1);

            string generatedQuery = new OSqlDeleteEdge()
                .Delete(profile)
                .From(vertex1)
                .To(vertex2)
                .ToString();

            string query =
                "DELETE EDGE FROM #8:0 TO #8:1";

            Assert.AreEqual(generatedQuery, query);
        }

        [Test]
        public void ShouldGenerateDeleteEdgeFromObjectToObjectQuery()
        {
            TestProfileClass profile = new TestProfileClass() ;

            TestProfileClass vertex1 = new TestProfileClass();
            vertex1.ORID = new ORID(8, 0);

            TestProfileClass vertex2 = new TestProfileClass();
            vertex2.ORID = new ORID(8, 1);

            string generatedQuery = new OSqlDeleteEdge()
                .Delete(profile)
                .From(vertex1)
                .To(vertex2)
                .ToString();

            string query =
                "DELETE EDGE FROM #8:0 TO #8:1";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
