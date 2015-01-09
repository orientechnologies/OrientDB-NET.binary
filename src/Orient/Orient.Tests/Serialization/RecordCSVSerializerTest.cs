using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    [TestClass]
    public class RecordCSVSerializerTest
    {
        [TestMethod]
        public void CanDeserializeARecordStringContainingNestedEmbeddedDocuments()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);
            const int innerInteger = 1;

            string recordString = "TestClassName@EmbeddedDocument:{\"NestedEmbeddedDocument\":{\"InnerInteger\":"+innerInteger+"}}";

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            Assert.AreEqual(innerInteger, actualDocument.GetField<int>("EmbeddedDocument.NestedEmbeddedDocument.InnerInteger"));
        }

        [TestMethod]
        public void CanDeserializeARecordStringContainingNestedArraysOfEmbeddedDocuments()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);
            const int innerInteger = 1;

            string recordString = "TestClassName@EmbeddedDocument:[{\"NestedEmbeddedDocument\":[{\"InnerInteger\":" + innerInteger + "}]}]";

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            var firstLevelEmbeddedDocuments = actualDocument.GetField<List<object>>("EmbeddedDocument").Cast<ODocument>().ToArray();
            var nestedDocuments = firstLevelEmbeddedDocuments[0].GetField<List<object>>("NestedEmbeddedDocument").Cast<ODocument>().ToArray();

            Assert.AreEqual(1, firstLevelEmbeddedDocuments.Length);
            Assert.AreEqual(1, nestedDocuments.Length);
            Assert.AreEqual(innerInteger, nestedDocuments[0].GetField<int>("InnerInteger"));
        }

        [TestMethod]
        public void CanDeserializeARecordIdInEmbeddedDocument()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);
            const int integerField = 1;
            var orid = new ORID(1,2);

            string recordString = "TestClassName@EmbeddedDocument:{\"RecordIdField\":"+orid+"},TestField:"+integerField;

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            Assert.AreEqual(orid, actualDocument.GetField<ORID>("EmbeddedDocument.RecordIdField"));
            Assert.AreEqual(integerField, actualDocument.GetField<int>("TestField"));
        }
    }
}
