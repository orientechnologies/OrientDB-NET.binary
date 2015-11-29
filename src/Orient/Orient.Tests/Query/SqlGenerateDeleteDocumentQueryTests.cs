﻿using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlGenerateDeleteDocumentQueryTests
    {
        [Fact]
        public void ShouldGenerateDeleteDocumentFromDocumentOClassNameQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestVertexClass";

            string generatedQuery = new OSqlDeleteDocument()
                .Delete(document)
                .ToString();

            string query =
                "DELETE FROM TestVertexClass";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateDeleteDocumentFromObjectOClassNameQuery()
        {
            TestProfileClass profile = new TestProfileClass();

            string generatedQuery = new OSqlDeleteDocument()
                .Delete(profile)
                .ToString();

            string query =
                "DELETE FROM TestProfileClass";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateDeleteDocumentFromDocumentOridQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestVertexClass";
            document.ORID = new ORID(8, 0);

            string generatedQuery = new OSqlDeleteDocument()
                .Delete(document)
                .ToString();

            string query =
                "DELETE FROM TestVertexClass " +
                "WHERE @rid = #8:0";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateDeleteDocumentFromObjectOridQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.ORID = new ORID(8, 0);

            string generatedQuery = new OSqlDeleteDocument()
                .Delete(profile)
                .ToString();

            string query =
                "DELETE FROM TestProfileClass " +
                "WHERE @rid = #8:0";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateDeleteDocumentClassWhereLimitQuery()
        {
            string generatedQuery = new OSqlDeleteDocument()
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
                "DELETE FROM TestProfileClass " +
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
