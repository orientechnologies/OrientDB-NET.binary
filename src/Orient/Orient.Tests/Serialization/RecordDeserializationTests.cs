using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Orient.Client;
using Orient.Client.API.Types;
using Orient.Client.Mapping;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    
    public class RecordDeserializationTests
    {
        private IRecordSerializer serializer;
        public RecordDeserializationTests()
        {
            serializer = RecordSerializerFactory.GetSerializer(ORecordFormat.ORecordDocument2csv);
        }
        [Fact]
        public void ShouldDeserializeDocSimpleExample()
        {
            string recordString = "Profile@nick:\"ThePresident\",follows:[],followers:[#10:5,#10:6],name:\"Barack\",surname:\"Obama\",location:#3:2,invitedBy:,salary_cloned:,salary:120.3f";
            var rawDocument = Encoding.UTF8.GetBytes(recordString);
            //ODocument document = ODocument.Deserialize(recordString);
            ODocument document = serializer.Deserialize(rawDocument, new ODocument());

            Assert.Equal(document.OClassName, "Profile");

            // check for fields existence
            Assert.Equal(document.HasField("nick"), true);
            Assert.Equal(document.HasField("follows"), true);
            Assert.Equal(document.HasField("followers"), true);
            Assert.Equal(document.HasField("name"), true);
            Assert.Equal(document.HasField("surname"), true);
            Assert.Equal(document.HasField("location"), true);
            Assert.Equal(document.HasField("invitedBy"), true);
            Assert.Equal(document.HasField("salary_cloned"), true);
            Assert.Equal(document.HasField("salary"), true);

            // check for fields values
            Assert.Equal(document.GetField<string>("nick"), "ThePresident");

            Assert.Equal(document.GetField<List<object>>("follows").Count, new List<object>().Count);


            List<ORID> recordFollowers = document.GetField<List<ORID>>("followers");
            List<ORID> followers = new List<ORID> { new ORID("#10:5"), new ORID("#10:6") };

            Assert.Equal(recordFollowers.Count, followers.Count);
            Assert.Equal(recordFollowers[0], followers[0]);
            Assert.Equal(recordFollowers[1], followers[1]);

            Assert.Equal(document.GetField<string>("name"), "Barack");
            Assert.Equal(document.GetField<string>("surname"), "Obama");
            Assert.Equal(document.GetField<ORID>("location"), new ORID("#3:2"));
            Assert.Equal(document.GetField<string>("invitedBy"), null);
            Assert.Equal(document.GetField<string>("salary_cloned"), null);
            Assert.Equal(document.GetField<float>("salary"), 120.3f);
        }

        [Fact]
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
            Assert.Equal(document.HasField("name"), true);
            Assert.Equal(document.HasField("id"), true);
            Assert.Equal(document.HasField("defaultClusterId"), true);
            Assert.Equal(document.HasField("clusterIds"), true);
            Assert.Equal(document.HasField("properties"), true);

            // check for fields values
            Assert.Equal(document.GetField<string>("name"), "ORole");
            Assert.Equal(document.GetField<int>("id"), 0);
            Assert.Equal(document.GetField<int>("defaultClusterId"), 3);
            Assert.Equal(document.GetField<List<int>>("clusterIds").Count, 1);
            Assert.Equal(document.GetField<List<int>>("clusterIds")[0], 3);

            List<DocComplexExampleEmbedded> loadedProperties = document.GetField<List<DocComplexExampleEmbedded>>("properties");
            List<DocComplexExampleEmbedded> properties = new List<DocComplexExampleEmbedded>();
            properties.Add(new DocComplexExampleEmbedded() { name = "mode", type = 17, offset = 0, mandatory = false, notNull = false, min = null, max = null, linkedClass = null, linkedType = null, index = null });
            properties.Add(new DocComplexExampleEmbedded() { name = "rules", type = 12, offset = 1, mandatory = false, notNull = false, min = null, max = null, linkedClass = null, linkedType = 17, index = null });

            Assert.Equal(loadedProperties.Count, properties.Count);

            for (int i = 0; i < loadedProperties.Count; i++)
            {
                Assert.Equal(loadedProperties[i].name, properties[i].name);
                Assert.Equal(loadedProperties[i].type, properties[i].type);
                Assert.Equal(loadedProperties[i].offset, properties[i].offset);
                Assert.Equal(loadedProperties[i].mandatory, properties[i].mandatory);
                Assert.Equal(loadedProperties[i].notNull, properties[i].notNull);
                Assert.Equal(loadedProperties[i].min, properties[i].min);
                Assert.Equal(loadedProperties[i].max, properties[i].max);
                Assert.Equal(loadedProperties[i].linkedClass, properties[i].linkedClass);
                Assert.Equal(loadedProperties[i].linkedType, properties[i].linkedType);
                Assert.Equal(loadedProperties[i].index, properties[i].index);
            }
        }

        [Fact]
        public void ShouldDeserializeDocBinary()
        {
            string recordString = "single:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,embedded:(binary:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_),array:[_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("single"), true);
            Assert.Equal(document.HasField("embedded"), true);
            Assert.Equal(document.HasField("embedded.binary"), true);
            Assert.Equal(document.HasField("array"), true);

            // check for fields values
            Assert.Equal(document.GetField<byte[]>("single").GetType(), typeof(byte[]));
            Assert.Equal(document.GetField<byte[]>("embedded.binary").GetType(), typeof(byte[]));

            List<byte[]> array = document.GetField<List<byte[]>>("array");
            Assert.Equal(array.Count, 2);
            Assert.Equal(array[0].GetType(), typeof(byte[]));
            Assert.Equal(array[0].GetType(), typeof(byte[]));
        }

        [Fact]
        public void ShouldDeserializeDateTime()
        {
            string recordString = "datetime:1296279468000t,date:1306281600000a,embedded:(datetime:1296279468000t,date:1306281600000a),array:[1296279468000t,1306281600000a]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("datetime"), true);
            Assert.Equal(document.HasField("date"), true);
            Assert.Equal(document.HasField("embedded.datetime"), true);
            Assert.Equal(document.HasField("embedded.date"), true);
            Assert.Equal(document.HasField("array"), true);

            // check for fields values
            Assert.Equal(document.GetField<DateTime>("datetime"), new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.Equal(document.GetField<DateTime>("date"), new DateTime(2011, 5, 25, 0, 0, 0));
            Assert.Equal(document.GetField<DateTime>("embedded.datetime"), new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.Equal(document.GetField<DateTime>("embedded.date"), new DateTime(2011, 5, 25, 0, 0, 0));

            List<DateTime> array = document.GetField<List<DateTime>>("array");
            Assert.Equal(array.Count, 2);
            Assert.Equal(array[0], new DateTime(2011, 1, 29, 5, 37, 48));
            Assert.Equal(array[1], new DateTime(2011, 5, 25, 0, 0, 0));
        }

        [Fact]
        public void ShouldDeserializeBoolean()
        {
            string recordString = "singleT:true,singleF:false,embedded:(singleT:true,singleF:false),array:[true,false]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("singleT"), true);
            Assert.Equal(document.HasField("singleF"), true);
            Assert.Equal(document.HasField("embedded.singleT"), true);
            Assert.Equal(document.HasField("embedded.singleF"), true);
            Assert.Equal(document.HasField("array"), true);

            // check for fields values
            Assert.Equal(document.GetField<bool>("singleT"), true);
            Assert.Equal(document.GetField<bool>("singleF"), false);
            Assert.Equal(document.GetField<bool>("embedded.singleT"), true);
            Assert.Equal(document.GetField<bool>("embedded.singleF"), false);

            List<bool> array = document.GetField<List<bool>>("array");
            Assert.Equal(array.Count, 2);
            Assert.Equal(array[0], true);
            Assert.Equal(array[1], false);
        }

        [Fact]
        public void ShouldDeserializeNull()
        {
            string recordString = "nick:,embedded:(nick:,joe:),joe:";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("nick"), true);
            Assert.Equal(document.HasField("embedded.nick"), true);
            Assert.Equal(document.HasField("embedded.joe"), true);
            Assert.Equal(document.HasField("joe"), true);

            // check for fields values
            Assert.Equal(document.GetField<string>("nick"), null);
            Assert.Equal(document.GetField<string>("embedded.nick"), null);
            Assert.Equal(document.GetField<string>("embedded.joe"), null);
            Assert.Equal(document.GetField<string>("joe"), null);
        }

        [Fact]
        public void ShouldDeserializeNumbers()
        {
            string recordString = "byte:123b,short:23456s,int:1543345,long:13243432455l,float:1234.432f,double:123123.4324d,bigdecimal:300.5c,embedded:(byte:123b,short:23456s,int:1543345,long:13243432455l,float:1234.432f,double:123123.4324d,bigdecimal:300.5c),array:[123b,23456s,1543345,13243432455l,1234.432f,123123.4324d,300.5c]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("byte"), true);
            Assert.Equal(document.HasField("short"), true);
            Assert.Equal(document.HasField("int"), true);
            Assert.Equal(document.HasField("long"), true);
            Assert.Equal(document.HasField("float"), true);
            Assert.Equal(document.HasField("double"), true);
            Assert.Equal(document.HasField("bigdecimal"), true);
            Assert.Equal(document.HasField("embedded.byte"), true);
            Assert.Equal(document.HasField("embedded.short"), true);
            Assert.Equal(document.HasField("embedded.int"), true);
            Assert.Equal(document.HasField("embedded.long"), true);
            Assert.Equal(document.HasField("embedded.float"), true);
            Assert.Equal(document.HasField("embedded.double"), true);
            Assert.Equal(document.HasField("embedded.bigdecimal"), true);
            Assert.Equal(document.HasField("array"), true);

            // check for fields values
            Assert.Equal(document.GetField<byte>("byte"), (byte)123);
            Assert.Equal(document.GetField<short>("short"), (short)23456);
            Assert.Equal(document.GetField<int>("int"), 1543345);
            Assert.Equal(document.GetField<long>("long"), 13243432455);
            Assert.Equal(document.GetField<float>("float"), 1234.432f);
            Assert.Equal(document.GetField<double>("double"), 123123.4324);
            Assert.Equal(document.GetField<decimal>("bigdecimal"), 300.5m);
            Assert.Equal(document.GetField<byte>("embedded.byte"), (byte)123);
            Assert.Equal(document.GetField<short>("embedded.short"), (short)23456);
            Assert.Equal(document.GetField<int>("embedded.int"), 1543345);
            Assert.Equal(document.GetField<long>("embedded.long"), 13243432455);
            Assert.Equal(document.GetField<float>("embedded.float"), 1234.432f);
            Assert.Equal(document.GetField<double>("embedded.double"), 123123.4324);
            Assert.Equal(document.GetField<decimal>("embedded.bigdecimal"), 300.5m);

            List<object> array = document.GetField<List<object>>("array");
            Assert.Equal(array.Count, 7);
            Assert.Equal(array[0], (byte)123);
            Assert.Equal(array[1], (short)23456);
            Assert.Equal(array[2], 1543345);
            Assert.Equal(array[3], 13243432455);
            Assert.Equal(array[4], 1234.432f);
            Assert.Equal(array[5], 123123.4324);
            Assert.Equal(array[6], 300.5m);
        }

        [Fact]
        public void ShouldDeserializeString()
        {
            string recordString = "simple:\"whoa this is awesome\",singleQuoted:\"a" + "\\" + "\"\",doubleQuoted:\"" + "\\" + "\"adsf" + "\\" + "\"\",twoBackslashes:\"" + "\\a" + "\\a" + "\"";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("simple"), true);
            Assert.Equal(document.HasField("singleQuoted"), true);
            Assert.Equal(document.HasField("doubleQuoted"), true);
            Assert.Equal(document.HasField("twoBackslashes"), true);

            // check for fields values
            Assert.Equal(document.GetField<string>("simple"), "whoa this is awesome");
            Assert.Equal(document.GetField<string>("singleQuoted"), "a\"");
            Assert.Equal(document.GetField<string>("doubleQuoted"), "\"adsf\"");
            Assert.Equal(document.GetField<string>("twoBackslashes"), "\\a\\a");
        }

        [Fact]
        public void ShouldDeserializeSimpleEmbeddedrecordsArray()
        {
            string recordString = "array:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"js3\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("array"), true);

            // check for fields values
            List<ODocument> array = document.GetField<List<ODocument>>("array");
            Assert.Equal(array.Count, 3);
            Assert.Equal(array[0].GetField<string>("joe1"), "js1");
            Assert.Equal(array[1].GetField<string>("joe2"), "js2");
            Assert.Equal(array[2].GetField<string>("joe3"), "js3");
        }

        [Fact]
        public void ShouldDeserializeComplexEmbeddedrecordsArray()
        {
            string recordString = "array:[(zak1:(nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"js3\")])),(zak2:(nick:[(joe4:\"js4\"),(joe5:\"js5\"),(joe6:\"js6\")]))]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("array"), true);

            // check for fields values
            List<ODocument> arrayOfZaks = document.GetField<List<ODocument>>("array");
            Assert.Equal(arrayOfZaks.Count, 2);

            List<ODocument> arrayOfJoes1 = arrayOfZaks[0].GetField<List<ODocument>>("zak1.nick");
            Assert.Equal(arrayOfJoes1.Count, 3);
            Assert.Equal(arrayOfJoes1[0].GetField<string>("joe1"), "js1");
            Assert.Equal(arrayOfJoes1[1].GetField<string>("joe2"), "js2");
            Assert.Equal(arrayOfJoes1[2].GetField<string>("joe3"), "js3");

            List<ODocument> arrayOfJoes2 = arrayOfZaks[1].GetField<List<ODocument>>("zak2.nick");
            Assert.Equal(arrayOfJoes2.Count, 3);
            Assert.Equal(arrayOfJoes2[0].GetField<string>("joe4"), "js4");
            Assert.Equal(arrayOfJoes2[1].GetField<string>("joe5"), "js5");
            Assert.Equal(arrayOfJoes2[2].GetField<string>("joe6"), "js6");
        }

        [Fact]
        public void ShouldDeserializeSingleAndListOfOrids()
        {
            string recordString = "single:#10:12345,list:[#11:123,#22:1234,#33:1234567]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("single"), true);
            Assert.Equal(document.HasField("list"), true);

            // check for fields values
            Assert.Equal(document.GetField<ORID>("single"), new ORID(10, 12345));
            List<ORID> collection = document.GetField<List<ORID>>("list");
            Assert.Equal(collection.Count, 3);
            Assert.Equal(collection[0], new ORID(11, 123));
            Assert.Equal(collection[1], new ORID(22, 1234));
            Assert.Equal(collection[2], new ORID(33, 1234567));
        }

        [Fact]
        public void ShouldDeserializeSingleItemToOneElementList()
        {
            string recordString = "single:#10:12345,list:[#11:123,#22:1234,#33:1234567]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("single"), true);
            Assert.Equal(document.HasField("list"), true);

            // check for fields values
            List<ORID> collection = document.GetField<List<ORID>>("single");
            Assert.Equal(collection.Count, 1);
            Assert.Equal(collection[0], new ORID(10, 12345));
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

        [Fact]
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
            Assert.Equal(document.HasField("single"), true);
            Assert.Equal(document.HasField("list"), true);

            var testObj = document.To<TestObject>();
            Assert.NotNull(testObj);
            Assert.NotNull(testObj.single);
            Assert.NotNull(testObj.list);
            Assert.Equal(1, testObj.single.Count);
            Assert.Equal(3, testObj.list.Count);
        }

        [Fact]
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

                    Assert.NotNull(testObj);
                    Assert.NotNull(testObj.single);
                    Assert.NotNull(testObj.list);
                    Assert.NotNull(testObj.ORID);
                    Assert.NotNull(testObj.Link);
                    Assert.Equal(1, testObj.single.Count);
                    Assert.Equal(3, testObj.list.Count);
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

        [Fact]
        public void TestDeserializeArray()
        {
            string recordString = "values:[1,2,3,4,5]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestArray> tm = TypeMapper<TestArray>.Instance;
            var t = new TestArray();
            tm.ToObject(document, t);

            Assert.NotNull(t.values);
            Assert.Equal(5, t.values.Length);
            Assert.Equal(3, t.values[2]);

        }

        [Fact]
        public void TestDeserializeList()
        {
            string recordString = "values:[1,2,3,4,5]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestList> tm = TypeMapper<TestList>.Instance;
            var t = new TestList();
            tm.ToObject(document, t);

            Assert.NotNull(t.values);
            Assert.Equal(5, t.values.Count);
            Assert.Equal(3, t.values[2]);

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

        [Fact]
        public void TestDeserializeSubObject()
        {
            string recordString = "TheThing:(Value:17,Text:\"blah\")";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasAThing> tm = TypeMapper<TestHasAThing>.Instance;
            var t = new TestHasAThing();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThing);
            Assert.Equal(17, t.TheThing.Value);
            Assert.Equal("blah", t.TheThing.Text);

        }

        class TestHasThings
        {
            public Thing[] TheThings { get; set; }
        }

        [Fact]
        public void TestDeserializeSubObjectArray()
        {
            string recordString = "TheThings:[(Value:17,Text:\"blah\"),(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings);
            Assert.Equal(2, t.TheThings.Length);
            Assert.Equal(18, t.TheThings[1].Value);
            Assert.Equal("foo", t.TheThings[1].Text);

        }

        [Fact]
        public void TestDeserializeSingleSubObjectArray()
        {
            string recordString = "TheThings:[(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings);
            Assert.Equal(1, t.TheThings.Length);
            Assert.Equal(18, t.TheThings[0].Value);
            Assert.Equal("foo", t.TheThings[0].Text);

        }

        [Fact]
        public void TestDeserializeEmptySubObjectArray()
        {
            string recordString = "TheThings:[]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings); // much easier for consumers to have a consistent behaviour - collections always created but empty, rather than having to test for nullness
            Assert.Equal(0, t.TheThings.Length);

        }

        class TestHasListThings
        {
            public List<Thing> TheThings { get; set; }
        }

        [Fact]
        public void TestDeserializeSubObjectList()
        {
            string recordString = "TheThings:[(Value:17,Text:\"blah\"),(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasListThings> tm = TypeMapper<TestHasListThings>.Instance;
            var t = new TestHasListThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings);
            Assert.Equal(2, t.TheThings.Count);
            Assert.Equal(18, t.TheThings[1].Value);
            Assert.Equal("foo", t.TheThings[1].Text);

        }

        [Fact]
        public void TestDeserializeSingleSubObjectList()
        {
            string recordString = "TheThings:[(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasListThings> tm = TypeMapper<TestHasListThings>.Instance;
            var t = new TestHasListThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings);
            Assert.Equal(1, t.TheThings.Count);

        }


        [Fact]
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
            Assert.Equal(document.HasField("single"), true);
            Assert.Equal(document.HasField("Link"), true);
            Assert.Equal(document.HasField("Value"), true);
            Assert.Equal(document.HasField("Text"), true);


            TypeMapper<TestObject> tm = TypeMapper<TestObject>.Instance;
            var testObj = new TestObject();
            tm.ToObject(document, testObj);

            Assert.Equal("#123:45", testObj.ORID.RID);

            Assert.Equal(17, testObj.Value);
            Assert.Equal("some text", testObj.Text);
            Assert.NotNull(testObj.Link);
            Assert.Equal("#11:123", testObj.Link.RID);
            Assert.NotNull(testObj);
            Assert.NotNull(testObj.single);
            Assert.NotNull(testObj.list);
            Assert.Equal(1, testObj.single.Count);
            Assert.Equal(3, testObj.list.Count);
        }

        [Fact]
        public void ShouldDeserializeSingleAndSetOfOrids()
        {
            string recordString = "single:#10:12345,set:<#11:123,#22:1234,#33:1234567>,singleSet:<#44:44>";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            // check for fields existence
            Assert.Equal(document.HasField("single"), true);
            Assert.Equal(document.HasField("set"), true);

            // check for fields values
            Assert.Equal(document.GetField<ORID>("single"), new ORID(10, 12345));
            HashSet<ORID> collection = document.GetField<HashSet<ORID>>("set");
            Assert.Equal(collection.Count, 3);
            Assert.True(collection.Contains(new ORID(11, 123)));
            Assert.True(collection.Contains(new ORID(22, 1234)));
            Assert.True(collection.Contains(new ORID(33, 1234567)));

            HashSet<ORID> singleSet = document.GetField<HashSet<ORID>>("singleSet");
            Assert.Equal(singleSet.Count, 1);
            Assert.True(singleSet.Contains(new ORID(44, 44)));
        }

        /*[Fact]
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
