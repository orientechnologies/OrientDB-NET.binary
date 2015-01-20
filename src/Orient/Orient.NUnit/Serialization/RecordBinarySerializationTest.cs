using System;
using System.Text;
using NUnit.Framework;
using Orient.Client;
using Orient.Client.API.Types;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    [TestFixture]
    public class RecordBinarySerializationTest
    {
        private IRecordSerializer serializer;
        TestDatabaseContext context;
        ODatabase database;

        [SetUp]
        public void Init()
        {
            context = new TestDatabaseContext();
            database = new ODatabase(TestConnection.GlobalTestDatabaseAlias);
            serializer = RecordSerializerFactory.GetSerializer(database);
        }

        [TearDown]
        public void Cleanup()
        {
            context.Dispose();
            context = null;
            database.Dispose();
            database = null;
        }


        [Test]
        public void testSimpleSerialization()
        {
            var clusterid = database
                .Create.Class("TestVertexClass")
                .Extends<OVertex>()
                .Run();

            var datetime = DateTime.Now;
            datetime = datetime.AddTicks(-(datetime.Ticks % TimeSpan.TicksPerSecond)); // ORIENTDB not count milliseconds

            OVertex createdVertex = database
                .Create.Vertex("TestVertexClass")
                .Set("foo", "foo string value")
                .Set("bar", 12345)
                .Set("_long", 1234566L)
                .Set("_short", (short)12)
                .Set("_float", 2.54f)
                .Set<Double>("_double", 1000234D)
                .Set<DateTime>("_datetime", datetime)
                //.Set<decimal>("_decimal", (decimal)10234.546) // Some problem with decimal not sure if this is a bug in product
                .Run();

            var loadedVertex = database.Load.ORID(createdVertex.ORID).Run();
            Assert.IsNotNull(loadedVertex);
            Assert.AreEqual("TestVertexClass", loadedVertex.OClassName);
            Assert.AreEqual("foo string value", loadedVertex.GetField<string>("foo"));
            Assert.AreEqual(12345, loadedVertex.GetField<int>("bar"));
            Assert.AreEqual(1234566L, loadedVertex.GetField<long>("_long"));
            Assert.AreEqual(12, loadedVertex.GetField<short>("_short"));
            Assert.AreEqual(2.54f, loadedVertex.GetField<float>("_float"));
            Assert.AreEqual(1000234D, loadedVertex.GetField<double>("_double"));
            Assert.AreEqual(datetime, loadedVertex.GetField<DateTime>("_datetime"));

        }
    }
}
