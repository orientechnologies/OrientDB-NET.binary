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
        public void CanDeserializeARecordStringContainingNestedEmbeddedMaps()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);
            const int innerInteger = 1;

            string recordString = "TestClassName@EmbeddedMap:{\"NestedEmbeddedMap\":{\"InnerInteger\":"+innerInteger+"}}";

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            Assert.AreEqual(innerInteger, actualDocument.GetField<int>("EmbeddedMap.NestedEmbeddedMap.InnerInteger"));
        }

        [TestMethod]
        public void CanDeserializeARecordStringContainingNestedArraysOfEmbeddedMaps()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);
            const int innerInteger = 1;

            string recordString = "TestClassName@EmbeddedMap:[{\"NestedEmbeddedMap\":[{\"InnerInteger\":" + innerInteger + "}]}]";

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            var firstLevelEmbeddedMaps = actualDocument.GetField<List<object>>("EmbeddedMap").Cast<ODocument>().ToArray();
            var nestedMaps = firstLevelEmbeddedMaps[0].GetField<List<object>>("NestedEmbeddedMap").Cast<ODocument>().ToArray();

            Assert.AreEqual(1, firstLevelEmbeddedMaps.Length);
            Assert.AreEqual(1, nestedMaps.Length);
            Assert.AreEqual(innerInteger, nestedMaps[0].GetField<int>("InnerInteger"));
        }

        [TestMethod]
        public void CanDeserializeARecordIdInEmbeddedMap()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);
            const int integerField = 1;
            var orid = new ORID(1,2);

            string recordString = "TestClassName@EmbeddedMap:{\"RecordIdField\":"+orid+"},TestField:"+integerField;

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            Assert.AreEqual(orid, actualDocument.GetField<ORID>("EmbeddedMap.RecordIdField"));
            Assert.AreEqual(integerField, actualDocument.GetField<int>("TestField"));
        }

        [TestMethod]
        public void CanDeserializeAnEmbeddedDocument()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);

            const string fieldValue = "field";
            const string recordString = "TestClassName@Map:{\"EmbeddedDocumentField\":(Name:\""+fieldValue+"\")}";

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            Assert.AreEqual(fieldValue, actualDocument.GetField<string>("Map.EmbeddedDocumentField.Name"));
        }
    }
}
