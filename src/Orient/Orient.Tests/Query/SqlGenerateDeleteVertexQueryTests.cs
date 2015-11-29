using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlGenerateDeleteVertexQueryTests
    {
        [Fact]
        public void ShouldGenerateDeleteVertexFromDocumentOridQuery()
        {
            ODocument document = new ODocument();
            document.ORID = new ORID(8, 0);

            string generatedQuery = new OSqlDeleteVertex()
                .Delete(document)
                .ToString();

            string query =
                "DELETE VERTEX #8:0";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateDeleteVertexFromObjectOridQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.ORID = new ORID(8, 0);

            string generatedQuery = new OSqlDeleteVertex()
                .Delete(profile)
                .ToString();

            string query =
                "DELETE VERTEX #8:0";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateDeleteVertexFromDocumentOClassNameQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestClass";

            string generatedQuery = new OSqlDeleteVertex()
                .Delete(document)
                .ToString();

            string query =
                "DELETE VERTEX TestClass";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateDeleteVertexFromObjectOClassNameQuery()
        {
            TestProfileClass profile = new TestProfileClass();

            string generatedQuery = new OSqlDeleteVertex()
                .Delete(profile)
                .ToString();

            string query =
                "DELETE VERTEX TestProfileClass";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateDeleteVertexClassWhereLimitQuery()
        {
            string generatedQuery = new OSqlDeleteVertex()
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
                "DELETE VERTEX TestProfileClass " +
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

            Assert.Equal(generatedQuery, query);
        }
    }
}
