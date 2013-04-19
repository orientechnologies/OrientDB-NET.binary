using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Query
{
    [TestClass]
    public class SqlGenerateUpdateQueryTests
    {
        [TestMethod]
        public void ShouldGenerateUpdateClassSetQuery()
        {
            string generatedUntypedQuery = new OSqlUpdate()
                .Class("TestVertexClass")
                .Set("foo", "new string value")
                .Set("bar", 54321)
                .ToString();

            ODocument document = new ODocument();
            document.SetField("foo", "new string value");
            document.SetField("bar", 54321);

            string generatedTypedQuery = new OSqlUpdate()
                .Class<TestVertexClass>()
                .Set(document)
                .ToString();

            string query =
                "UPDATE TestVertexClass " +
                "SET foo = 'new string value', " +
                "bar = 54321";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateUpdateRecordSetQuery()
        {
            ODocument vertex = new ODocument()
                .SetField("@ORID", new ORID(8, 0));

            string generatedQuery = new OSqlUpdate()
                .Record(vertex)
                .Set("foo", "new string value")
                .Set("bar", 54321)
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "SET foo = 'new string value', " +
                "bar = 54321";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateUpdateDocumentQuery()
        {
            ODocument vertex = new ODocument()
                .SetField("@ORID", new ORID(8, 0))
                .SetField("foo", "new string value")
                .SetField("bar", 54321);

            string generatedQuery = new OSqlUpdate()
                .Document(vertex)
                .ToString();

            string query =
                "UPDATE #8:0 " +
                "SET foo = 'new string value', " +
                "bar = 54321";

            Assert.AreEqual(generatedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateUpdateClusterSetQuery()
        {
            string generatedUntypedQuery = new OSqlUpdate()
                .Cluster("TestVertexClass")
                .Set("foo", "new string value")
                .Set("bar", 54321)
                .ToString();

            ODocument document = new ODocument();
            document.SetField("foo", "new string value");
            document.SetField("bar", 54321);

            string generatedTypedQuery = new OSqlUpdate()
                .Cluster<TestVertexClass>()
                .Set(document)
                .ToString();

            string query =
                "UPDATE cluster:TestVertexClass " +
                "SET foo = 'new string value', " +
                "bar = 54321";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateUpdateAddQuery()
        {
            string generatedUntypedQuery = new OSqlUpdate()
                .Class("TestVertexClass")
                .Add("foo", "foo string value")
                .Add("bar", 12345)
                .ToString();

            string generatedTypedQuery = new OSqlUpdate()
                .Class<TestVertexClass>()
                .Add("foo", "foo string value")
                .Add("bar", 12345)
                .ToString();

            string query =
                "UPDATE TestVertexClass " +
                "ADD foo = 'foo string value', " +
                "bar = 12345";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }

        [TestMethod]
        public void ShouldGenerateUpdateRemoveQuery()
        {
            string generatedUntypedQuery = new OSqlUpdate()
                .Class("TestVertexClass")
                .Remove("foo")
                .Remove("bar")
                .ToString();

            string generatedTypedQuery = new OSqlUpdate()
                .Class<TestVertexClass>()
                .Remove("foo")
                .Remove("bar")
                .ToString();

            string query =
                "UPDATE TestVertexClass " +
                "REMOVE foo, bar";

            Assert.AreEqual(generatedUntypedQuery, query);
            Assert.AreEqual(generatedTypedQuery, query);
        }
    }
}
