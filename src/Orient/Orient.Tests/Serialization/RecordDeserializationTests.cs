using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Serialization
{
    [TestClass]
    public class RecordDeserializationTests
    {
        [TestMethod]
        public void ShouldDeserializeDocSimpleExample()
        {
            string recordString = "Profile@nick:\"ThePresident\",follows:[],followers:[#10:5,#10:6],name:\"Barack\",surname:\"Obama\",location:#3:2,invitedBy:,salary_cloned:,salary:120.3f";

            ORecord record = new ORecord();
            record.FromString(recordString);

            Assert.AreEqual(record.ClassName, "Profile");

            // check for fields existence
            Assert.AreEqual(record.HasField("nick"), true);
            Assert.AreEqual(record.HasField("follows"), true);
            Assert.AreEqual(record.HasField("followers"), true);
            Assert.AreEqual(record.HasField("name"), true);
            Assert.AreEqual(record.HasField("surname"), true);
            Assert.AreEqual(record.HasField("location"), true);
            Assert.AreEqual(record.HasField("invitedBy"), true);
            Assert.AreEqual(record.HasField("salary_cloned"), true);
            Assert.AreEqual(record.HasField("salary"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<string>("nick"), "ThePresident");

            Assert.AreEqual(record.GetField<List<object>>("follows").Count, new List<object>().Count);


            List<ORID> recordFollowers = record.GetField<List<ORID>>("followers");
            List<ORID> followers = new List<ORID> { new ORID("#10:5"), new ORID("#10:6") };

            Assert.AreEqual(recordFollowers.Count, followers.Count);
            Assert.AreEqual(recordFollowers[0], followers[0]);
            Assert.AreEqual(recordFollowers[1], followers[1]);
            
            Assert.AreEqual(record.GetField<string>("name"), "Barack");
            Assert.AreEqual(record.GetField<string>("surname"), "Obama");
            Assert.AreEqual(record.GetField<ORID>("location"), new ORID("#3:2"));
            Assert.AreEqual(record.GetField<string>("invitedBy"), null);
            Assert.AreEqual(record.GetField<string>("salary_cloned"), null);
            Assert.AreEqual(record.GetField<float>("salary"), 120.3f);
        }

        [TestMethod]
        public void ShouldDeserializeDocComplexExample()
        {
            string recordString = 
                "name:\"ORole\"," + 
                "id:0," +
                "defaultClusterId:3," +
                "clusterIds:[3]," +
                "properties:[" +
                    "(name:\"mode\",type:17,offset:0,mandatory:false,notNull:false,min:,max:,linkedClass:,linkedType:,index:)," +
                    "(name:\"rules\",type:12,offset:1,mandatory:false,notNull:false,min:,max:,linkedClass:,linkedType:17,index:)" +
                "]";

            ORecord record = new ORecord();
            record.FromString(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("name"), true);
            Assert.AreEqual(record.HasField("id"), true);
            Assert.AreEqual(record.HasField("defaultClusterId"), true);
            Assert.AreEqual(record.HasField("clusterIds"), true);
            Assert.AreEqual(record.HasField("properties"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<string>("name"), "ORole");
            Assert.AreEqual(record.GetField<int>("id"), 0);
            Assert.AreEqual(record.GetField<int>("defaultClusterId"), 3);
            Assert.AreEqual(record.GetField<List<int>>("clusterIds").Count, 1);
            Assert.AreEqual(record.GetField<List<int>>("clusterIds")[0], 3);

            List<DocComplexExampleEmbedded> loadedProperties = record.GetField<List<DocComplexExampleEmbedded>>("properties");
            List<DocComplexExampleEmbedded> properties = new List<DocComplexExampleEmbedded>();
            properties.Add(new DocComplexExampleEmbedded() { name = "mode", type = 17, offset = 0, mandatory = false, notNull = false, min = null, max = null, linkedClass = null, linkedType = null, index = null });
            properties.Add(new DocComplexExampleEmbedded() { name = "rules", type = 12, offset = 1, mandatory = false, notNull = false, min = null, max = null, linkedClass = null, linkedType = 17, index = null });

            Assert.AreEqual(loadedProperties.Count, properties.Count);

            for (int i = 0; i < loadedProperties.Count; i++)
            {
                Assert.AreEqual(loadedProperties[i].name, properties[i].name);
                Assert.AreEqual(loadedProperties[i].type, properties[i].type);
                Assert.AreEqual(loadedProperties[i].offset, properties[i].offset);
                Assert.AreEqual(loadedProperties[i].mandatory, properties[i].mandatory);
                Assert.AreEqual(loadedProperties[i].notNull, properties[i].notNull);
                Assert.AreEqual(loadedProperties[i].min, properties[i].min);
                Assert.AreEqual(loadedProperties[i].max, properties[i].max);
                Assert.AreEqual(loadedProperties[i].linkedClass, properties[i].linkedClass);
                Assert.AreEqual(loadedProperties[i].linkedType, properties[i].linkedType);
                Assert.AreEqual(loadedProperties[i].index, properties[i].index);
            }
        }

        [TestMethod]
        public void ShouldDeserializeDocBinary()
        {
            string recordString = "single:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,embedded:(binary:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_),array:[_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_]";

            ORecord record = new ORecord();
            record.FromString(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("single"), true);
            Assert.AreEqual(record.HasField("embedded"), true);
            Assert.AreEqual(record.HasField("embedded.binary"), true);
            Assert.AreEqual(record.HasField("array"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<byte[]>("single").GetType(), typeof(byte[]));
            Assert.AreEqual(record.GetField<byte[]>("embedded.binary").GetType(), typeof(byte[]));

            List<byte[]> array = record.GetField<List<byte[]>>("array");
            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual(array[0].GetType(), typeof(byte[]));
            Assert.AreEqual(array[0].GetType(), typeof(byte[]));
        }
    }

    class DocComplexExampleEmbedded
    {
        public string name { get; set; }
        public int type { get; set; }
        public int offset { get; set; }
        public bool mandatory { get; set; }
        public bool notNull { get; set; }
        public int? min { get; set; }
        public int? max { get; set; }
        public object linkedClass { get; set; }
        public int? linkedType { get; set; }
        public int? index { get; set; }
    }
}
