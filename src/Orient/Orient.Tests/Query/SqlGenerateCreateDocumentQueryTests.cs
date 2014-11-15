using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;
using Orient.Client.API.Query;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateCreateDocumentQueryTests
    {
        [TestMethod]
        public void ShouldGenerateInsertDocumentQuery()
        {
            ODocument document = new ODocument();
            document.OClassName = "TestClass";
            document
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlCreateDocument()
                .Document(document)
                .ToString();

            string query =
                "INSERT INTO TestClass " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateInsertIntoClusterSetQuery()
        {
            ODocument document = new ODocument()
                .SetField("foo", "foo string value")
                .SetField("bar", 12345);

            string generatedQuery = new OSqlCreateDocument()
                .Document("TestClass")
                .Cluster("TestCluster")
                .Set(document)
                .ToString();

            string query =
                "INSERT INTO TestClass " +
                "CLUSTER TestCluster " +
                "SET foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedQuery, query);
        }
    }
}
