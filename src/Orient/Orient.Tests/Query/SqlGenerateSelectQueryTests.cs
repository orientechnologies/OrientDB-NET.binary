using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateSelectQueryTests
    {
        [TestMethod]
        public void ShouldGenerate_Select_Also_First_Nth_As_Query()
        {
            string generatedUntypedQuery = new OSqlSelect()
                .Select("foo").As("Foo")
                .Also("bar").As("Bar")
                .Also("baz").First().As("Baz")
                .Also("baq").Nth(0).As("Baq")
                .From("OGraphVertex")
                .ToString();

            string generatedTypedQuery = new OSqlSelect()
                .Select("foo").As("Foo")
                .Also("bar").As("Bar")
                .Also("baz").First().As("Baz")
                .Also("baq").Nth(0).As("Baq")
                .From<OGraphVertex>()
                .ToString();

            string query =
                "SELECT foo AS Foo, " +
                "bar AS Bar, " +
                "first(baz) AS Baz, " +
                "baq[0] AS Baq " +
                "FROM OGraphVertex";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerate_Select_Where_And_Or_Like_IsNull_Conditions_Query()
        {
            string generatedUntypedQuery = new OSqlSelect()
                .Select()
                .From("OGraphVertex")
                .Where("foo").Equals("foo string")
                .And("bar").NotEquals(12345)
                .Or("baz").LesserEqual(10)
                .Or("baq").GreaterEqual(50)
                .Or("f1").Like("fulltext%")
                .Or("f2").IsNull()
                .ToString();

            string generatedTypedQuery = new OSqlSelect()
                .Select()
                .From<OGraphVertex>()
                .Where("foo").Equals("foo string")
                .And("bar").NotEquals(12345)
                .Or("baz").LesserEqual(10)
                .Or("baq").GreaterEqual(50)
                .Or("f1").Like("fulltext%")
                .Or("f2").IsNull()
                .ToString();

            string query =
                "SELECT " +
                "FROM OGraphVertex " +
                "WHERE foo = 'foo string' " +
                "AND bar != 12345 " +
                "OR baz <= 10 " +
                "OR baq >= 50 " +
                "OR f1 LIKE 'fulltext%' " +
                "OR f2 IS NULL";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerate_Select_Where_Contains_Query()
        {
            string generatedUntypedQuery = new OSqlSelect()
                .Select()
                .From("OGraphVertex")
                .Where("foo").Contains("english")
                .And("bar").Contains("foo", 123)
                .ToString();

            string generatedTypedQuery = new OSqlSelect()
                .Select()
                .From<OGraphVertex>()
                .Where("foo").Contains("english")
                .And("bar").Contains("foo", 123)
                .ToString();

            string query =
                "SELECT " +
                "FROM OGraphVertex " +
                "WHERE foo CONTAINS 'english' " +
                "AND bar CONTAINS (foo = 123)";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateSelectFromDocumentQuery()
        {
            ODocument document = new ODocument()
                .SetField("@ORID", new ORID(8, 0));

            string generatedQuery = new OSqlSelect()
                .Select("foo", "bar")
                .From(document)
                .Where("foo").Equals("english")
                .And("bar").Equals(123)
                .ToString();

            string query =
                "SELECT foo, bar " +
                "FROM #8:0 " +
                "WHERE foo = 'english' " +
                "AND bar = 123";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
