using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    [TestClass]
    public class RecordBinarySerializationTest
    {
        [TestMethod]
        public void testSimpleSerialization()
        {
            var document = new ODocument();
            document.OClassName = "TestsClass";
            document.SetField<string>("name", "name");
            document.SetField<int>("age", 20);
            document.SetField<short>("youngAge", 20);
            document.SetField<long>("oldAge", 20);
            document.SetField<float>("heigth", 12.5f);

            //TODO: Add other supported fields

            var serDocument = RecordBinarySerializer.Serialize(document);
        }
    }
}
