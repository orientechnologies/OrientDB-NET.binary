using System;
using System.Text;
using Xunit;
using Orient.Client;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    
    public class RecordBinaryDeserializationTest : IDisposable
    {
        private IRecordSerializer serializer;
        TestDatabaseContext context;
        ODatabase database;
        
        public RecordBinaryDeserializationTest()
        {
            context = new TestDatabaseContext();
            database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            serializer = RecordSerializerFactory.GetSerializer(database);

            database
                .Create
                .Class("TestClass")
                .Run();

            database
                .Create
                .Property("_date", OType.Date)
                .Class("TestClass")
                .Run();
        }

        public void Dispose()
        {
            context.Dispose();
            context = null;
            database.Dispose();
            database = null;
        }

        //[Fact]
        public void ShouldDeserializeWholeStructure()
        {
            /*
                The whole record is structured in three main segments
                +---------------+------------------+---------------+-------------+
                | version:byte   | className:string | header:byte[] | data:byte[]  |
                +---------------+------------------+---------------+-------------+
             */


            //byte version = 0;
            byte[] className = Encoding.UTF8.GetBytes("TestClass");
            byte[] header = new byte[0];
            byte[] data = new byte[0];

            //string serString = "ABJUZXN0Q2xhc3MpAAAAEQDI/wE=";
            string serString1 = "AAxQZXJzb24EaWQAAABEBwhuYW1lAAAAaQcOc3VybmFtZQAAAHAHEGJpcnRoZGF5AAAAdwYQY2hpbGRyZW4AAAB9AQBIZjk1M2VjNmMtNGYyMC00NDlhLWE2ODQtYjQ2ODkxNmU4NmM3DEJpbGx5MQxNYXllczGUlfWVo1IC/wE=";

            var document = new ODocument();
            document.OClassName = "TestClass";
            document.SetField<DateTime>("_date", DateTime.Now);

            var createdDocument = database
                .Create
                .Document(document)
                .Run();

            Assert.Equal(document.GetField<DateTime>("_date").Date, createdDocument.GetField<DateTime>("eeee"));
            var serBytes1 = Convert.FromBase64String(serString1);
            var doc = serializer.Deserialize(serBytes1, new ODocument());
        }

        //[Fact]
        public void ShouldSerializeDocumnet()
        {
            //string serString = "ABJUZXN0Q2xhc3MpAAAAEQDI/wE=";
            ODocument document = new ODocument();
            document.OClassName = "TestClass";
            document.SetField<DateTime>("eeee", new DateTime(635487552000000000));

            var str = Convert.ToBase64String(serializer.Serialize(document));
        }
    }
}
