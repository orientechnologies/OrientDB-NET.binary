using System;
using Xunit;
using Orient.Client;

namespace Orient.Tests.Query
{
    
    public class SqlGenerateInsertQueryTests
    {
        [Fact]
        public void ShouldGenerateInsertDocumentQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestClass";
            document
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlInsert()
                .Insert(document)
                .ToString();

            string query =
                "INSERT INTO TestClass " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateInsertObjectQuery()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.Name = "Johny";
            profile.Surname = "Bravo";

            string generatedQuery = new OSqlInsert()
                .Insert(profile)
                .ToString();

            string query =
                "INSERT INTO TestProfileClass " +
                "SET Name = 'Johny', " +
                "Surname = 'Bravo'";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateInsertDocumentIntoQuery()
        {
            ODocument document = new ODocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlInsert()
                .Insert(document)
                .Into("TestClass")
                .ToString();

            string query =
                "INSERT INTO TestClass " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateInsertDocumentIntoClusterQuery()
        {
            ODocument document = new ODocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlInsert()
                .Insert(document)
                .Into("TestClass")
                .Cluster("TestCluster")
                .ToString();

            string query =
                "INSERT INTO TestClass " +
                "CLUSTER TestCluster " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
        public void ShouldGenerateInsertIntoClusterSetQuery()
        {
            string generatedQuery = new OSqlInsert()
                .Into("TestClass")
                .Cluster("TestCluster")
                .Set("foo", "foo string value")
                .Set("bar", 12345)
                .ToString();

            string query =
                "INSERT INTO TestClass " +
                "CLUSTER TestCluster " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.Equal(generatedQuery, query);
        }
    }
}
