﻿using System;
using Xunit;
using Orient.Client;
using Orient.Client.API.Query;

namespace Orient.Tests.Query
{
    
    public class SqlGenerateCreateDocumentQueryTests
    {
        [Fact]
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

            Assert.Equal(generatedQuery, query);
        }

        [Fact]
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

            Assert.Equal(generatedQuery, query);
        }
    }
}
