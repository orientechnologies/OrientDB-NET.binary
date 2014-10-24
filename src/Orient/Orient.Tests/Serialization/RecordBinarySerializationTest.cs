using System;
using System.Text;
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
            //OClient.Serializer = ORecordFormat.ORecordDocument2csv;
            serializer = RecordSerializerFactory.GetSerializer(OClient.Serializer);
        }

        [TestMethod]
        public void testSimpleSerialization()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    byte[] rawRecord;
                    unchecked
                    {
                        rawRecord = new byte[] { 0, 30, 84, 101, 115, 116, 86, 101, 114, 116, 101, 120, 67, 108, 97, 115, 115, 14, 95, 100, 111, 117, 98, 108, 101, 0, 0, 0, 31, 5, 0, 65, 46, (byte)-122, 84, 0, 0, 0, 0 };
                    }


                    //var recordSerialized = "TestsClass@name:\"name\",age:20,youngAge:20s,oldAge:20l,heigth:12.5f";
                    var clusterid = database
                        .Create.Class("TestVertexClass")
                        .Extends<OVertex>()
                        .Run();

                    // var odoc = database.Command("create vertex TestVertexClass set _double=1000234d").ToDocument();
                    var datetime = DateTime.Now;

                    OVertex createdVertex = database
                        .Create.Vertex("TestVertexClass")
                        .Set("foo", "foo string value")
                        .Set("bar", 12345)
                        .Set("_long", 1234566L)
                        .Set("_short", (short)12)
                        .Set("_float", 2.54f)
                        .Set<Double>("_double", 1000234D)
                        .Set<DateTime>("_datetime", datetime)
                        .Set<decimal>("_decimal", (decimal)10234.546)
                        .Run();


                    var loadedVertex = database.Load.ORID(new ORID("#11:0")).Run();

                    //TODO: Add other supported fields
                    // var serDocument = serializer.Serialize(document);
                    var d = serializer.Deserialize(rawRecord, new ODocument());

                    var document = new ODocument();
                    document.OClassName = "TestVertexClass";
                    document.SetField<int>("bar", 12345);

                    Assert.Fail();
                }
            }
        }
    }
}
