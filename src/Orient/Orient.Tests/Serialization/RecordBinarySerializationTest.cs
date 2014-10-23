using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;
using Orient.Client.API.Types;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    [TestClass]
    public class RecordBinarySerializationTest
    {
        private IRecordSerializer serializer;

        [TestInitialize]
        public void Init()
        {
            OClient.Serializer = ORecordFormat.ORecordDocument2csv;
            serializer = RecordSerializerFactory.GetSerializer(OClient.Serializer);
        }

        [TestMethod]
        public void testSimpleSerialization()
        {
            var recordSerialized = "TestsClass@name:\"name\",age:20,youngAge:20s,oldAge:20l,heigth:12.5f";

            var document = new ODocument();
            document.OClassName = "TestsClass";
            document.SetField<string>("name", "name");
            document.SetField<int>("age", 20);
            document.SetField<short>("youngAge", 20);
            document.SetField<long>("oldAge", 20);
            document.SetField<float>("heigth", 12.5f);

            //TODO: Add other supported fields

            var serDocument = serializer.Serialize(document);
            Assert.AreEqual(recordSerialized, serDocument);
        }
    }
}
