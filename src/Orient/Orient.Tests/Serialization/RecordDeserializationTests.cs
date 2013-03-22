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

            ORecord record = new ORecord(recordString);

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

            ORecord record = new ORecord(recordString);

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

            ORecord record = new ORecord(recordString);

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

        [TestMethod]
        public void ShouldDeserializeDateTime()
        {
            string recordString = "datetime:1296279468000t,date:1306281600000a,embedded:(datetime:1296279468000t,date:1306281600000a),array:[1296279468000t,1306281600000a]";

            ORecord record = new ORecord(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("datetime"), true);
            Assert.AreEqual(record.HasField("date"), true);
            Assert.AreEqual(record.HasField("embedded.datetime"), true);
            Assert.AreEqual(record.HasField("embedded.date"), true);
            Assert.AreEqual(record.HasField("array"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<DateTime>("datetime"), new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.AreEqual(record.GetField<DateTime>("date"), new DateTime(2011, 5, 25, 0, 0, 0));
            Assert.AreEqual(record.GetField<DateTime>("embedded.datetime"), new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.AreEqual(record.GetField<DateTime>("embedded.date"), new DateTime(2011, 5, 25, 0, 0, 0));

            List<DateTime> array = record.GetField<List<DateTime>>("array");
            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual(array[0], new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.AreEqual(array[1], new DateTime(2011, 5, 25, 0, 0, 0));
        }

        [TestMethod]
        public void ShouldDeserializeBoolean()
        {
            string recordString = "singleT:true,singleF:false,embedded:(singleT:true,singleF:false),array:[true,false]";

            ORecord record = new ORecord(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("singleT"), true);
            Assert.AreEqual(record.HasField("singleF"), true);
            Assert.AreEqual(record.HasField("embedded.singleT"), true);
            Assert.AreEqual(record.HasField("embedded.singleF"), true);
            Assert.AreEqual(record.HasField("array"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<bool>("singleT"), true);
            Assert.AreEqual(record.GetField<bool>("singleF"), false);
            Assert.AreEqual(record.GetField<bool>("embedded.singleT"), true);
            Assert.AreEqual(record.GetField<bool>("embedded.singleF"), false);
            
            List<bool> array = record.GetField<List<bool>>("array");
            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual(array[0], true);
            Assert.AreEqual(array[1], false);
        }

        [TestMethod]
        public void ShouldDeserializeNull()
        {
            string recordString = "nick:,embedded:(nick:,joe:),joe:";

            ORecord record = new ORecord(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("nick"), true);
            Assert.AreEqual(record.HasField("embedded.nick"), true);
            Assert.AreEqual(record.HasField("embedded.joe"), true);
            Assert.AreEqual(record.HasField("joe"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<string>("nick"), null);
            Assert.AreEqual(record.GetField<string>("embedded.nick"), null);
            Assert.AreEqual(record.GetField<string>("embedded.joe"), null);
            Assert.AreEqual(record.GetField<string>("joe"), null);
        }

        [TestMethod]
        public void ShouldDeserializeNumbers()
        {
            string recordString = "byte:123b,short:23456s,int:1543345,long:13243432455l,float:1234.432f,double:123123.4324d,bigdecimal:300.5c,embedded:(byte:123b,short:23456s,int:1543345,long:13243432455l,float:1234.432f,double:123123.4324d,bigdecimal:300.5c),array:[123b,23456s,1543345,13243432455l,1234.432f,123123.4324d,300.5c]";

            ORecord record = new ORecord(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("byte"), true);
            Assert.AreEqual(record.HasField("short"), true);
            Assert.AreEqual(record.HasField("int"), true);
            Assert.AreEqual(record.HasField("long"), true);
            Assert.AreEqual(record.HasField("float"), true);
            Assert.AreEqual(record.HasField("double"), true);
            Assert.AreEqual(record.HasField("bigdecimal"), true);
            Assert.AreEqual(record.HasField("embedded.byte"), true);
            Assert.AreEqual(record.HasField("embedded.short"), true);
            Assert.AreEqual(record.HasField("embedded.int"), true);
            Assert.AreEqual(record.HasField("embedded.long"), true);
            Assert.AreEqual(record.HasField("embedded.float"), true);
            Assert.AreEqual(record.HasField("embedded.double"), true);
            Assert.AreEqual(record.HasField("embedded.bigdecimal"), true);
            Assert.AreEqual(record.HasField("array"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<byte>("byte"), (byte)123);
            Assert.AreEqual(record.GetField<short>("short"), (short)23456);
            Assert.AreEqual(record.GetField<int>("int"), 1543345);
            Assert.AreEqual(record.GetField<long>("long"), 13243432455);
            Assert.AreEqual(record.GetField<float>("float"), 1234.432f);
            Assert.AreEqual(record.GetField<double>("double"), 123123.4324);
            Assert.AreEqual(record.GetField<decimal>("bigdecimal"), 300.5m);
            Assert.AreEqual(record.GetField<byte>("embedded.byte"), (byte)123);
            Assert.AreEqual(record.GetField<short>("embedded.short"), (short)23456);
            Assert.AreEqual(record.GetField<int>("embedded.int"), 1543345);
            Assert.AreEqual(record.GetField<long>("embedded.long"), 13243432455);
            Assert.AreEqual(record.GetField<float>("embedded.float"), 1234.432f);
            Assert.AreEqual(record.GetField<double>("embedded.double"), 123123.4324);
            Assert.AreEqual(record.GetField<decimal>("embedded.bigdecimal"), 300.5m);

            List<object> array = record.GetField<List<object>>("array");
            Assert.AreEqual(array.Count, 7);
            Assert.AreEqual(array[0], (byte)123);
            Assert.AreEqual(array[1], (short)23456);
            Assert.AreEqual(array[2], 1543345);
            Assert.AreEqual(array[3], 13243432455);
            Assert.AreEqual(array[4], 1234.432f);
            Assert.AreEqual(array[5], 123123.4324);
            Assert.AreEqual(array[6], 300.5m);
        }

        [TestMethod]
        public void ShouldDeserializeString()
        {
            string recordString = "simple:\"whoa this is awesome\",singleQuoted:\"a" + "\\" + "\"\",doubleQuoted:\"" + "\\" + "\"adsf" + "\\" + "\"\",twoBackslashes:\"" + "\\a" + "\\a" + "\"";

            ORecord record = new ORecord(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("simple"), true);
            Assert.AreEqual(record.HasField("singleQuoted"), true);
            Assert.AreEqual(record.HasField("doubleQuoted"), true);
            Assert.AreEqual(record.HasField("twoBackslashes"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<string>("simple"), "whoa this is awesome");
            Assert.AreEqual(record.GetField<string>("singleQuoted"), "a\"");
            Assert.AreEqual(record.GetField<string>("doubleQuoted"), "\"adsf\"");
            Assert.AreEqual(record.GetField<string>("twoBackslashes"), "\\a\\a");
        }

        [TestMethod]
        public void ShouldDeserializeSimpleEmbeddedrecordsArray()
        {
            string recordString = "array:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"js3\")]";

            ORecord record = new ORecord(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("array"), true);

            // check for fields values
            List<ODataObject> array = record.GetField<List<ODataObject>>("array");
            Assert.AreEqual(array.Count, 3);
            Assert.AreEqual(array[0].Get<string>("joe1"), "js1");
            Assert.AreEqual(array[1].Get<string>("joe2"), "js2");
            Assert.AreEqual(array[2].Get<string>("joe3"), "js3");
        }

        [TestMethod]
        public void ShouldDeserializeComplexEmbeddedrecordsArray()
        {
            string recordString = "array:[(zak1:(nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"js3\")])),(zak2:(nick:[(joe4:\"js4\"),(joe5:\"js5\"),(joe6:\"js6\")]))]";

            ORecord record = new ORecord(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("array"), true);

            // check for fields values
            List<ODataObject> arrayOfZaks = record.GetField<List<ODataObject>>("array");
            Assert.AreEqual(arrayOfZaks.Count, 2);

            List<ODataObject> arrayOfJoes1 = arrayOfZaks[0].Get<List<ODataObject>>("zak1.nick");
            Assert.AreEqual(arrayOfJoes1.Count, 3);
            Assert.AreEqual(arrayOfJoes1[0].Get<string>("joe1"), "js1");
            Assert.AreEqual(arrayOfJoes1[1].Get<string>("joe2"), "js2");
            Assert.AreEqual(arrayOfJoes1[2].Get<string>("joe3"), "js3");

            List<ODataObject> arrayOfJoes2 = arrayOfZaks[1].Get<List<ODataObject>>("zak2.nick");
            Assert.AreEqual(arrayOfJoes2.Count, 3);
            Assert.AreEqual(arrayOfJoes2[0].Get<string>("joe4"), "js4");
            Assert.AreEqual(arrayOfJoes2[1].Get<string>("joe5"), "js5");
            Assert.AreEqual(arrayOfJoes2[2].Get<string>("joe6"), "js6");
        }

        [TestMethod]
        public void ShouldDeserializeSingleAndCollectionOfOrids()
        {
            string recordString = "single:#10:12345,collection:[#11:123,#22:1234,#33:1234567]";

            ORecord record = new ORecord(recordString);

            // check for fields existence
            Assert.AreEqual(record.HasField("single"), true);
            Assert.AreEqual(record.HasField("collection"), true);

            // check for fields values
            Assert.AreEqual(record.GetField<ORID>("single"), new ORID(10, 12345));
            List<ORID> collection = record.GetField<List<ORID>>("collection");
            Assert.AreEqual(collection.Count, 3);
            Assert.AreEqual(collection[0], new ORID(11, 123));
            Assert.AreEqual(collection[1], new ORID(22, 1234));
            Assert.AreEqual(collection[2], new ORID(33, 1234567));
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
