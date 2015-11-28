using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Orient.Client;
using Orient.Client.API.Types;
using Orient.Client.Mapping;
using Orient.Client.Protocol.Serializers;

namespace Orient.Tests.Serialization
{
    
    public class RecordDocumentSerializationTests
    {
        private IRecordSerializer serializer;
        public RecordDocumentSerializationTests()
        {
            serializer = RecordSerializerFactory.GetSerializer(ORecordFormat.ORecordDocument2csv);
        }

        [Fact]
        public void ShouldParseClassToDocument()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.ORID = new ORID(8, 0);
            profile.OVersion = 1;
            profile.OClassId = 8;
            profile.Name = "Johny";
            profile.Surname = "Bravo";

            ODocument document = profile.ToDocument();

            Assert.Equal(document.ORID, profile.ORID);
            Assert.Equal(document.OVersion, profile.OVersion);
            Assert.Equal(document.OClassId, profile.OClassId);
            Assert.Equal(document.OClassName, typeof(TestProfileClass).Name);

            Assert.Equal(document.GetField<string>("Name"), profile.Name);
            Assert.Equal(document.GetField<string>("Surname"), profile.Surname);
        }

        [Fact]
        public void ShouldParseClassToDocumentWithExplicitOClassName()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.ORID = new ORID(8, 0);
            profile.OVersion = 1;
            profile.OClassId = 8;
            profile.OClassName = "OtherProfileClass";
            profile.Name = "Johny";
            profile.Surname = "Bravo";

            ODocument document = profile.ToDocument();

            Assert.Equal(document.ORID, profile.ORID);
            Assert.Equal(document.OVersion, profile.OVersion);
            Assert.Equal(document.OClassId, profile.OClassId);
            Assert.Equal(document.OClassName, "OtherProfileClass");

            Assert.Equal(document.GetField<string>("Name"), profile.Name);
            Assert.Equal(document.GetField<string>("Surname"), profile.Surname);
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
        public void TestSerializeArray()
        {
            string recordString = "TestArray@values:[1,2,3,4,5]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestArray> tm = TypeMapper<TestArray>.Instance;
            var t = new TestArray();
            tm.ToObject(document, t);

            Assert.NotNull(t.values);
            Assert.Equal(5, t.values.Length);
            Assert.Equal(3, t.values[2]);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);
        }

        [Fact]
        public void TestSerializeList()
        {
            string recordString = "TestList@values:[1,2,3,4,5]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());


            TypeMapper<TestList> tm = TypeMapper<TestList>.Instance;
            var t = new TestList();
            tm.ToObject(document, t);

            Assert.NotNull(t.values);
            Assert.Equal(5, t.values.Count);
            Assert.Equal(3, t.values[2]);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);
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
        public void TestSerializeSubObject()
        {
            string recordString = "TestHasAThing@TheThing:(Value:17,Text:\"blah\")";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());


            TypeMapper<TestHasAThing> tm = TypeMapper<TestHasAThing>.Instance;
            var t = new TestHasAThing();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThing);
            Assert.Equal(17, t.TheThing.Value);
            Assert.Equal("blah", t.TheThing.Text);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);

        }

        class TestHasThings
        {
            public Thing[] TheThings { get; set; }
        }

        [Fact]
        public void TestSerializeSubObjectArray()
        {
            string recordString = "TestHasThings@TheThings:[(Value:17,Text:\"blah\"),(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());


            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings);
            Assert.Equal(2, t.TheThings.Length);
            Assert.Equal(18, t.TheThings[1].Value);
            Assert.Equal("foo", t.TheThings[1].Text);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);
        }

        [Fact]
        public void TestSerializeSingleSubObjectArray()
        {
            string recordString = "TestHasThings@TheThings:[(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings);
            Assert.Equal(1, t.TheThings.Length);
            Assert.Equal(18, t.TheThings[0].Value);
            Assert.Equal("foo", t.TheThings[0].Text);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);
        }

        [Fact]
        public void TestSerializeEmptySubObjectArray()
        {
            string recordString = "TestHasThings@TheThings:[]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings); // much easier for consumers to have a consistent behaviour - collections always created but empty, rather than having to test for nullness
            Assert.Equal(0, t.TheThings.Length);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);
        }

        class TestHasListThings
        {
            public List<Thing> TheThings { get; set; }
        }

        [Fact]
        public void TestSerializeSubObjectList()
        {
            string recordString = "TestHasListThings@TheThings:[(Value:17,Text:\"blah\"),(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasListThings> tm = TypeMapper<TestHasListThings>.Instance;
            var t = new TestHasListThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings);
            Assert.Equal(2, t.TheThings.Count);
            Assert.Equal(18, t.TheThings[1].Value);
            Assert.Equal("foo", t.TheThings[1].Text);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);
        }

        [Fact]
        public void TestSerializeSingleSubObjectList()
        {
            string recordString = "TestHasListThings@TheThings:[(Value:18,Text:\"foo\")]";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument());

            TypeMapper<TestHasListThings> tm = TypeMapper<TestHasListThings>.Instance;
            var t = new TestHasListThings();
            tm.ToObject(document, t);

            Assert.NotNull(t.TheThings);
            Assert.Equal(1, t.TheThings.Count);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);
        }

        class TestObject
        {
            public int Value { get; set; }
            public string Text { get; set; }
            public ORID Link { get; set; }
            public List<ORID> single { get; set; }
            public List<ORID> list { get; set; }

            public ORID ORID { get; set; }
            public byte[] data { get; set; }
        }

        [Fact]
        public void TestSerializationMapping()
        {
            // important if you use ordered edges, since if more than 1 they appear as a list, if only one then as a single object, ie
            //    db.Create.Class<Person>().Extends("V").Run();
            //    db.Command("create property Person.in_FriendOf ANY");
            //    db.Command("alter property Person.in_FriendOf custom ordered=true");

            string recordString = "TestObject@Value:17,Text:\"some text\",Link:#11:123,single:[#10:12345],list:[#11:123,#22:1234,#33:1234567],data:_AQIDBAU=_";

            var rawRecord = Encoding.UTF8.GetBytes(recordString);
            ODocument document = serializer.Deserialize(rawRecord, new ODocument()); 
            
            document.ORID = new ORID("#123:45");
            Assert.Equal(document.HasField("single"), true);
            Assert.Equal(document.HasField("Link"), true);
            Assert.Equal(document.HasField("Value"), true);
            Assert.Equal(document.HasField("Text"), true);
            Assert.Equal(document.HasField("data"), true);


            TypeMapper<TestObject> tm = TypeMapper<TestObject>.Instance;
            var testObj = new TestObject();
            tm.ToObject(document, testObj);

            Assert.Equal("#123:45", testObj.ORID.RID);
            Assert.True(testObj.data.SequenceEqual(new byte[] { 1, 2, 3, 4, 5 }));

            Assert.Equal(17, testObj.Value);
            Assert.Equal("some text", testObj.Text);
            Assert.NotNull(testObj.Link);
            Assert.Equal("#11:123", testObj.Link.RID);
            Assert.NotNull(testObj);
            Assert.NotNull(testObj.single);
            Assert.NotNull(testObj.list);
            Assert.Equal(1, testObj.single.Count);
            Assert.Equal(3, testObj.list.Count);

            ODocument newODocument = ODocument.ToDocument(testObj);
            var serialized = Encoding.UTF8.GetString(serializer.Serialize(newODocument));
            Assert.Equal(recordString, serialized);
        }

        [Fact]
        public void ShouldSerializeSingleAndSetOfOrids()
        {
            string recordString = "Something@single:#10:12345,set:<#11:123,#22:1234,#33:1234567>,singleSet:<#44:44>";

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

            var serialized = Encoding.UTF8.GetString(serializer.Serialize(document));
            Assert.Equal(recordString, serialized);
        }


    }
}
