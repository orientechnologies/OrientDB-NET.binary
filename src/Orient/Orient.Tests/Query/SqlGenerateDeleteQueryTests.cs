using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateDeleteQueryTests
    {
        [TestMethod]
        public void ShouldGenerateDeleteFromDocumentOClassNameQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestVertexClass";

            string generatedQuery = new OSqlDelete()
                .Delete(document)
                .ToString();

            string query =
                "DELETE FROM TestVertexClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateDeleteFromObjectOClassNameQuery()
        {
            TestProfileClass profile = new TestProfileClass();

            string generatedQuery = new OSqlDelete()
                .Delete(profile)
                .ToString();

            string query =
                "DELETE FROM TestProfileClass";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateDeleteFromDocumentOridQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestVertexClass";
            document.ORID = new ORID(8, 0);

            string generatedQuery = new OSqlDelete()
                .Delete(document)
                .ToString();

            string query =
                "DELETE FROM TestVertexClass " +
                "WHERE @rid = #8:0";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateDeleteFromObjectOridQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.ORID = new ORID(8, 0);

            string generatedQuery = new OSqlDelete()
                .Delete(profile)
                .ToString();

            string query =
                "DELETE FROM TestProfileClass " +
                "WHERE @rid = #8:0";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateDeleteWhereLimitQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestClass";

            string generatedQuery = new OSqlDelete()
                .Delete(document)
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
                "DELETE FROM TestClass " +
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
    }
}
