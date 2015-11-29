using System.Collections.Generic;
using System.Linq;
using Xunit;
using Orient.Client;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    
    public class RecordCSVSerializerTest
    {
        [Fact]
        public void CanDeserializeARecordStringContainingNestedEmbeddedMaps()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);
            const int innerInteger = 1;

            string recordString = "TestClassName@EmbeddedMap:{\"NestedEmbeddedMap\":{\"InnerInteger\":" + innerInteger + "}}";

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            Assert.Equal(innerInteger, actualDocument.GetField<int>("EmbeddedMap.NestedEmbeddedMap.InnerInteger"));
        }

        [Fact]
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

            Assert.Equal(1, firstLevelEmbeddedMaps.Length);
            Assert.Equal(1, nestedMaps.Length);
            Assert.Equal(innerInteger, nestedMaps[0].GetField<int>("InnerInteger"));
        }

        [Fact]
        public void CanDeserializeARecordIdInEmbeddedMap()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);
            const int integerField = 1;
            var orid = new ORID(1, 2);

            string recordString = "TestClassName@EmbeddedMap:{\"RecordIdField\":" + orid + "},TestField:" + integerField;

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            Assert.Equal(orid, actualDocument.GetField<ORID>("EmbeddedMap.RecordIdField"));
            Assert.Equal(integerField, actualDocument.GetField<int>("TestField"));
        }

        [Fact]
        public void CanDeserializeAnEmbeddedDocument()
        {
            // Arrange
            var recordCsvSerializer = new RecordCSVSerializer(null);

            const string fieldValue = "field";
            const string recordString = "TestClassName@Map:{\"EmbeddedDocumentField\":(Name:\"" + fieldValue + "\")}";

            // Act
            var actualDocument = recordCsvSerializer.Deserialize(recordString);

            // Assert
            Assert.Equal(fieldValue, actualDocument.GetField<string>("Map.EmbeddedDocumentField.Name"));
        }
    }
}
