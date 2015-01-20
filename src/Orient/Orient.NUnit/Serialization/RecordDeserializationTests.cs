using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Orient.Client;
using Orient.Client.API.Types;
using Orient.Client.Mapping;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    [TestFixture]
    public class RecordDeserializationTests
    {
        private IRecordSerializer serializer;
        [SetUp]
        public void Init()
        {
            serializer = RecordSerializerFactory.GetSerializer(ORecordFormat.ORecordDocument2csv);
        }
        [Test]
        public void ShouldDeserializeDocSimpleExample()
        {
            string recordString = "Profile@nick:\"ThePresident\",follows:[],followers:[#10:5,#10:6],name:\"Barack\",surname:\"Obama\",location:#3:2,invitedBy:,salary_cloned:,salary:120.3f";
            var rawDocument = Encoding.UTF8.GetBytes(recordString);
            //ODocument document = ODocument.Deserialize(recordString);
            ODocument document = serializer.Deserialize(rawDocument, new ODocument());

            Assert.AreEqual(document.OClassName, "Profile");

            // check for fields existence
            Assert.AreEqual(document.HasField("nick"), true);
            Assert.AreEqual(document.HasField("follows"), true);
            Assert.AreEqual(document.HasField("followers"), true);
            Assert.AreEqual(document.HasField("name"), true);
            Assert.AreEqual(document.HasField("surname"), true);
            Assert.AreEqual(document.HasField("location"), true);
            Assert.AreEqual(document.HasField("invitedBy"), true);
            Assert.AreEqual(document.HasField("salary_cloned"), true);
            Assert.AreEqual(document.HasField("salary"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<string>("nick"), "ThePresident");

            Assert.AreEqual(document.GetField<List<object>>("follows").Count, new List<object>().Count);


            List<ORID> recordFollowers = document.GetField<List<ORID>>("followers");
            List<ORID> followers = new List<ORID> { new ORID("#10:5"), new ORID("#10:6") };

            Assert.AreEqual(recordFollowers.Count, followers.Count);
            Assert.AreEqual(recordFollowers[0], followers[0]);
            Assert.AreEqual(recordFollowers[1], followers[1]);

            Assert.AreEqual(document.GetField<string>("name"), "Barack");
            Assert.AreEqual(document.GetField<string>("surname"), "Obama");
            Assert.AreEqual(document.GetField<ORID>("location"), new ORID("#3:2"));
            Assert.AreEqual(document.GetField<string>("invitedBy"), null);
            Assert.AreEqual(document.GetField<string>("salary_cloned"), null);
            Assert.AreEqual(document.GetField<float>("salary"), 120.3f);
        }

        [Test]
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
            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("name"), true);
            Assert.AreEqual(document.HasField("id"), true);
            Assert.AreEqual(document.HasField("defaultClusterId"), true);
            Assert.AreEqual(document.HasField("clusterIds"), true);
            Assert.AreEqual(document.HasField("properties"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<string>("name"), "ORole");
            Assert.AreEqual(document.GetField<int>("id"), 0);
            Assert.AreEqual(document.GetField<int>("defaultClusterId"), 3);
            Assert.AreEqual(document.GetField<List<int>>("clusterIds").Count, 1);
            Assert.AreEqual(document.GetField<List<int>>("clusterIds")[0], 3);

            List<DocComplexExampleEmbedded> loadedProperties = document.GetField<List<DocComplexExampleEmbedded>>("properties");
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

        [Test]
        public void ShouldDeserializeDocBinary()
        {
            string recordString = "single:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,embedded:(binary:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_),array:[_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("single"), true);
            Assert.AreEqual(document.HasField("embedded"), true);
            Assert.AreEqual(document.HasField("embedded.binary"), true);
            Assert.AreEqual(document.HasField("array"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<byte[]>("single").GetType(), typeof(byte[]));
            Assert.AreEqual(document.GetField<byte[]>("embedded.binary").GetType(), typeof(byte[]));

            List<byte[]> array = document.GetField<List<byte[]>>("array");
            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual(array[0].GetType(), typeof(byte[]));
            Assert.AreEqual(array[0].GetType(), typeof(byte[]));
        }

        [Test]
        public void ShouldDeserializeDateTime()
        {
            string recordString = "datetime:1296279468000t,date:1306281600000a,embedded:(datetime:1296279468000t,date:1306281600000a),array:[1296279468000t,1306281600000a]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("datetime"), true);
            Assert.AreEqual(document.HasField("date"), true);
            Assert.AreEqual(document.HasField("embedded.datetime"), true);
            Assert.AreEqual(document.HasField("embedded.date"), true);
            Assert.AreEqual(document.HasField("array"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<DateTime>("datetime"), new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.AreEqual(document.GetField<DateTime>("date"), new DateTime(2011, 5, 25, 0, 0, 0));
            Assert.AreEqual(document.GetField<DateTime>("embedded.datetime"), new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.AreEqual(document.GetField<DateTime>("embedded.date"), new DateTime(2011, 5, 25, 0, 0, 0));

            List<DateTime> array = document.GetField<List<DateTime>>("array");
            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual(array[0], new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.AreEqual(array[1], new DateTime(2011, 5, 25, 0, 0, 0));
        }

        [Test]
        public void ShouldDeserializeBoolean()
        {
            string recordString = "singleT:true,singleF:false,embedded:(singleT:true,singleF:false),array:[true,false]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("singleT"), true);
            Assert.AreEqual(document.HasField("singleF"), true);
            Assert.AreEqual(document.HasField("embedded.singleT"), true);
            Assert.AreEqual(document.HasField("embedded.singleF"), true);
            Assert.AreEqual(document.HasField("array"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<bool>("singleT"), true);
            Assert.AreEqual(document.GetField<bool>("singleF"), false);
            Assert.AreEqual(document.GetField<bool>("embedded.singleT"), true);
            Assert.AreEqual(document.GetField<bool>("embedded.singleF"), false);

            List<bool> array = document.GetField<List<bool>>("array");
            Assert.AreEqual(array.Count, 2);
            Assert.AreEqual(array[0], true);
            Assert.AreEqual(array[1], false);
        }

        [Test]
        public void ShouldDeserializeNull()
        {
            string recordString = "nick:,embedded:(nick:,joe:),joe:";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("nick"), true);
            Assert.AreEqual(document.HasField("embedded.nick"), true);
            Assert.AreEqual(document.HasField("embedded.joe"), true);
            Assert.AreEqual(document.HasField("joe"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<string>("nick"), null);
            Assert.AreEqual(document.GetField<string>("embedded.nick"), null);
            Assert.AreEqual(document.GetField<string>("embedded.joe"), null);
            Assert.AreEqual(document.GetField<string>("joe"), null);
        }

        [Test]
        public void ShouldDeserializeNumbers()
        {
            string recordString = "byte:123b,short:23456s,int:1543345,long:13243432455l,float:1234.432f,double:123123.4324d,bigdecimal:300.5c,embedded:(byte:123b,short:23456s,int:1543345,long:13243432455l,float:1234.432f,double:123123.4324d,bigdecimal:300.5c),array:[123b,23456s,1543345,13243432455l,1234.432f,123123.4324d,300.5c]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("byte"), true);
            Assert.AreEqual(document.HasField("short"), true);
            Assert.AreEqual(document.HasField("int"), true);
            Assert.AreEqual(document.HasField("long"), true);
            Assert.AreEqual(document.HasField("float"), true);
            Assert.AreEqual(document.HasField("double"), true);
            Assert.AreEqual(document.HasField("bigdecimal"), true);
            Assert.AreEqual(document.HasField("embedded.byte"), true);
            Assert.AreEqual(document.HasField("embedded.short"), true);
            Assert.AreEqual(document.HasField("embedded.int"), true);
            Assert.AreEqual(document.HasField("embedded.long"), true);
            Assert.AreEqual(document.HasField("embedded.float"), true);
            Assert.AreEqual(document.HasField("embedded.double"), true);
            Assert.AreEqual(document.HasField("embedded.bigdecimal"), true);
            Assert.AreEqual(document.HasField("array"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<byte>("byte"), (byte)123);
            Assert.AreEqual(document.GetField<short>("short"), (short)23456);
            Assert.AreEqual(document.GetField<int>("int"), 1543345);
            Assert.AreEqual(document.GetField<long>("long"), 13243432455);
            Assert.AreEqual(document.GetField<float>("float"), 1234.432f);
            Assert.AreEqual(document.GetField<double>("double"), 123123.4324);
            Assert.AreEqual(document.GetField<decimal>("bigdecimal"), 300.5m);
            Assert.AreEqual(document.GetField<byte>("embedded.byte"), (byte)123);
            Assert.AreEqual(document.GetField<short>("embedded.short"), (short)23456);
            Assert.AreEqual(document.GetField<int>("embedded.int"), 1543345);
            Assert.AreEqual(document.GetField<long>("embedded.long"), 13243432455);
            Assert.AreEqual(document.GetField<float>("embedded.float"), 1234.432f);
            Assert.AreEqual(document.GetField<double>("embedded.double"), 123123.4324);
            Assert.AreEqual(document.GetField<decimal>("embedded.bigdecimal"), 300.5m);

            List<object> array = document.GetField<List<object>>("array");
            Assert.AreEqual(array.Count, 7);
            Assert.AreEqual(array[0], (byte)123);
            Assert.AreEqual(array[1], (short)23456);
            Assert.AreEqual(array[2], 1543345);
            Assert.AreEqual(array[3], 13243432455);
            Assert.AreEqual(array[4], 1234.432f);
            Assert.AreEqual(array[5], 123123.4324);
            Assert.AreEqual(array[6], 300.5m);
        }

        [Test]
        public void ShouldDeserializeString()
        {
            string recordString = "simple:\"whoa this is awesome\",singleQuoted:\"a" + "\\" + "\"\",doubleQuoted:\"" + "\\" + "\"adsf" + "\\" + "\"\",twoBackslashes:\"" + "\\a" + "\\a" + "\"";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("simple"), true);
            Assert.AreEqual(document.HasField("singleQuoted"), true);
            Assert.AreEqual(document.HasField("doubleQuoted"), true);
            Assert.AreEqual(document.HasField("twoBackslashes"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<string>("simple"), "whoa this is awesome");
            Assert.AreEqual(document.GetField<string>("singleQuoted"), "a\"");
            Assert.AreEqual(document.GetField<string>("doubleQuoted"), "\"adsf\"");
            Assert.AreEqual(document.GetField<string>("twoBackslashes"), "\\a\\a");
        }

        [Test]
        public void ShouldDeserializeSimpleEmbeddedrecordsArray()
        {
            string recordString = "array:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"js3\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("array"), true);

            // check for fields values
            List<ODocument> array = document.GetField<List<ODocument>>("array");
            Assert.AreEqual(array.Count, 3);
            Assert.AreEqual(array[0].GetField<string>("joe1"), "js1");
            Assert.AreEqual(array[1].GetField<string>("joe2"), "js2");
            Assert.AreEqual(array[2].GetField<string>("joe3"), "js3");
        }

        [Test]
        public void ShouldDeserializeComplexEmbeddedrecordsArray()
        {
            string recordString = "array:[(zak1:(nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"js3\")])),(zak2:(nick:[(joe4:\"js4\"),(joe5:\"js5\"),(joe6:\"js6\")]))]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("array"), true);

            // check for fields values
            List<ODocument> arrayOfZaks = document.GetField<List<ODocument>>("array");
            Assert.AreEqual(arrayOfZaks.Count, 2);

            List<ODocument> arrayOfJoes1 = arrayOfZaks[0].GetField<List<ODocument>>("zak1.nick");
            Assert.AreEqual(arrayOfJoes1.Count, 3);
            Assert.AreEqual(arrayOfJoes1[0].GetField<string>("joe1"), "js1");
            Assert.AreEqual(arrayOfJoes1[1].GetField<string>("joe2"), "js2");
            Assert.AreEqual(arrayOfJoes1[2].GetField<string>("joe3"), "js3");

            List<ODocument> arrayOfJoes2 = arrayOfZaks[1].GetField<List<ODocument>>("zak2.nick");
            Assert.AreEqual(arrayOfJoes2.Count, 3);
            Assert.AreEqual(arrayOfJoes2[0].GetField<string>("joe4"), "js4");
            Assert.AreEqual(arrayOfJoes2[1].GetField<string>("joe5"), "js5");
            Assert.AreEqual(arrayOfJoes2[2].GetField<string>("joe6"), "js6");
        }

        [Test]
        public void ShouldDeserializeSingleAndListOfOrids()
        {
            string recordString = "single:#10:12345,list:[#11:123,#22:1234,#33:1234567]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("single"), true);
            Assert.AreEqual(document.HasField("list"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<ORID>("single"), new ORID(10, 12345));
            List<ORID> collection = document.GetField<List<ORID>>("list");
            Assert.AreEqual(collection.Count, 3);
            Assert.AreEqual(collection[0], new ORID(11, 123));
            Assert.AreEqual(collection[1], new ORID(22, 1234));
            Assert.AreEqual(collection[2], new ORID(33, 1234567));
        }

        [Test]
        public void ShouldDeserializeSingleItemToOneElementList()
        {
            string recordString = "single:#10:12345,list:[#11:123,#22:1234,#33:1234567]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("single"), true);
            Assert.AreEqual(document.HasField("list"), true);

            // check for fields values
            List<ORID> collection = document.GetField<List<ORID>>("single");
            Assert.AreEqual(collection.Count, 1);
            Assert.AreEqual(collection[0], new ORID(10, 12345));
        }

        class TestObject
        {
            public int Value { get; set; }
            public string Text { get; set; }
            public ORID Link { get; set; }
            public List<ORID> single { get; set; }
            public List<ORID> list { get; set; }

            public ORID ORID { get; set; }
        }

        [Test]
        public void ShouldDeserializeSingleItemToOneElementListFieldOfObject()
        {
            // important if you use ordered edges, since if more than 1 they appear as a list, if only one then as a single object, ie
            //    db.Create.Class<Person>().Extends("V").Run();
            //    db.Command("create property Person.in_FriendOf ANY");
            //    db.Command("alter property Person.in_FriendOf custom ordered=true");

            string recordString = "single:#10:12345,list:[#11:123,#22:1234,#33:1234567],ORID:#10:123,Link:#10:234,Value:17";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("single"), true);
            Assert.AreEqual(document.HasField("list"), true);

            var testObj = document.To<TestObject>();
            Assert.IsNotNull(testObj);
            Assert.IsNotNull(testObj.single);
            Assert.IsNotNull(testObj.list);
            Assert.AreEqual(1, testObj.single.Count);
            Assert.AreEqual(3, testObj.list.Count);
        }

        [Test]
        public void ShouldDeserializeSingleItemToOneElementListFieldOfObjectViaDB()
        {
            using (TestDatabaseContext testContext = new TestDatabaseContext())
            {
                using (ODatabase database = new ODatabase(TestConnection.GlobalTestDatabaseAlias))
                {
                    // important if you use ordered edges, since if more than 1 they appear as a list, if only one then as a single object, ie
                    //    db.Create.Class<Person>().Extends("V").Run();
                    //    db.Command("create property Person.in_FriendOf ANY");
                    //    db.Command("alter property Person.in_FriendOf custom ordered=true");

                    string recordString = "single:#10:12345,list:[#11:123,#22:1234,#33:1234567],ORID:#10:123,Link:#10:234,Value:17";

                    var rawRecord = Encoding.UTF8.GetBytes(recordString);
                    ODocument document = serializer.Deserialize(rawRecord, new ODocument());

                    var vertex = database.Create.Vertex("V").Set(document).Run();

                    var loaded = database.Load.ORID(vertex.ORID).Run();


                    var testObj = loaded.To<TestObject>();

                    Assert.IsNotNull(testObj);
                    Assert.IsNotNull(testObj.single);
                    Assert.IsNotNull(testObj.list);
                    Assert.IsNotNull(testObj.ORID);
                    Assert.IsNotNull(testObj.Link);
                    Assert.AreEqual(1, testObj.single.Count);
                    Assert.AreEqual(3, testObj.list.Count);
                }
            }
        }

        class TestArray
        {
            public int[] values { get; set; }
        }

        class TestList
        {
            public List<int> values { get; set; }
        }

        [Test]
        public void TestDeserializeArray()
        {
            string recordString = "values:[1,2,3,4,5]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestArray> tm = TypeMapper<TestArray>.Instance;
            var t = new TestArray();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.values);
            Assert.AreEqual(5, t.values.Length);
            Assert.AreEqual(3, t.values[2]);

        }

        [Test]
        public void TestDeserializeList()
        {
            string recordString = "values:[1,2,3,4,5]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestList> tm = TypeMapper<TestList>.Instance;
            var t = new TestList();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.values);
            Assert.AreEqual(5, t.values.Count);
            Assert.AreEqual(3, t.values[2]);

        }

        class Thing
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        class TestHasAThing
        {
            public Thing TheThing { get; set; }
        }

        [Test]
        public void TestDeserializeSubObject()
        {
            string recordString = "TheThing:(Value:17,Text:\"blah\")";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasAThing> tm = TypeMapper<TestHasAThing>.Instance;
            var t = new TestHasAThing();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThing);
            Assert.AreEqual(17, t.TheThing.Value);
            Assert.AreEqual("blah", t.TheThing.Text);

        }

        class TestHasThings
        {
            public Thing[] TheThings { get; set; }
        }

        [Test]
        public void TestDeserializeSubObjectArray()
        {
            string recordString = "TheThings:[(Value:17,Text:\"blah\"),(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings);
            Assert.AreEqual(2, t.TheThings.Length);
            Assert.AreEqual(18, t.TheThings[1].Value);
            Assert.AreEqual("foo", t.TheThings[1].Text);

        }

        [Test]
        public void TestDeserializeSingleSubObjectArray()
        {
            string recordString = "TheThings:[(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings);
            Assert.AreEqual(1, t.TheThings.Length);
            Assert.AreEqual(18, t.TheThings[0].Value);
            Assert.AreEqual("foo", t.TheThings[0].Text);

        }

        [Test]
        public void TestDeserializeEmptySubObjectArray()
        {
            string recordString = "TheThings:[]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings, "much easier for consumers to have a consistent behaviour - collections always created but empty, rather than having to test for nullness");
            Assert.AreEqual(0, t.TheThings.Length);

        }

        class TestHasListThings
        {
            public List<Thing> TheThings { get; set; }
        }

        [Test]
        public void TestDeserializeSubObjectList()
        {
            string recordString = "TheThings:[(Value:17,Text:\"blah\"),(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasListThings> tm = TypeMapper<TestHasListThings>.Instance;
            var t = new TestHasListThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings);
            Assert.AreEqual(2, t.TheThings.Count);
            Assert.AreEqual(18, t.TheThings[1].Value);
            Assert.AreEqual("foo", t.TheThings[1].Text);

        }

        [Test]
        public void TestDeserializeSingleSubObjectList()
        {
            string recordString = "TheThings:[(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasListThings> tm = TypeMapper<TestHasListThings>.Instance;
            var t = new TestHasListThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings);
            Assert.AreEqual(1, t.TheThings.Count);

        }


        [Test]
        public void TestDeserializationMapping()
        {
            // important if you use ordered edges, since if more than 1 they appear as a list, if only one then as a single object, ie
            //    db.Create.Class<Person>().Extends("V").Run();
            //    db.Command("create property Person.in_FriendOf ANY");
            //    db.Command("alter property Person.in_FriendOf custom ordered=true");

            string recordString = "single:#10:12345,list:[#11:123,#22:1234,#33:1234567],Link:#11:123,Value:17,Text:\"some text\"";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument()); 
            
            document.ORID = new ORID("#123:45");
            Assert.AreEqual(document.HasField("single"), true);
            Assert.AreEqual(document.HasField("Link"), true);
            Assert.AreEqual(document.HasField("Value"), true);
            Assert.AreEqual(document.HasField("Text"), true);


            TypeMapper<TestObject> tm = TypeMapper<TestObject>.Instance;
            var testObj = new TestObject();
            tm.ToObject(document, testObj);

            Assert.AreEqual("#123:45", testObj.ORID.RID);

            Assert.AreEqual(17, testObj.Value);
            Assert.AreEqual("some text", testObj.Text);
            Assert.IsNotNull(testObj.Link);
            Assert.AreEqual("#11:123", testObj.Link.RID);
            Assert.IsNotNull(testObj);
            Assert.IsNotNull(testObj.single);
            Assert.IsNotNull(testObj.list);
            Assert.AreEqual(1, testObj.single.Count);
            Assert.AreEqual(3, testObj.list.Count);
        }

        [Test]
        public void ShouldDeserializeSingleAndSetOfOrids()
        {
            string recordString = "single:#10:12345,set:<#11:123,#22:1234,#33:1234567>,singleSet:<#44:44>";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.AreEqual(document.HasField("single"), true);
            Assert.AreEqual(document.HasField("set"), true);

            // check for fields values
            Assert.AreEqual(document.GetField<ORID>("single"), new ORID(10, 12345));
            HashSet<ORID> collection = document.GetField<HashSet<ORID>>("set");
            Assert.AreEqual(collection.Count, 3);
            Assert.IsTrue(collection.Contains(new ORID(11, 123)));
            Assert.IsTrue(collection.Contains(new ORID(22, 1234)));
            Assert.IsTrue(collection.Contains(new ORID(33, 1234567)));

            HashSet<ORID> singleSet = document.GetField<HashSet<ORID>>("singleSet");
            Assert.AreEqual(singleSet.Count, 1);
            Assert.IsTrue(singleSet.Contains(new ORID(44, 44)));
        }

        /*[Test]
        public void ShouldDeserializeMapExample()
        {
            string recordString = "ORole@name:\"reader\",inheritedRole:,mode:0,rules:{\"database\":2,\"database.cluster.internal\":2,\"database.cluster.orole\":2,\"database.cluster.ouser\":2,\"database.class.*\":2,\"database.cluster.*\":2,\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}";

            ODocument document = ODocument.Deserialize(recordString);

            // check for fields existence

            // check for fields values
        }*/
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
