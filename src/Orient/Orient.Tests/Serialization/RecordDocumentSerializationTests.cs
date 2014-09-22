using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;
using Orient.Client.Mapping;

namespace Orient.Tests.Serialization
{
    [TestClass]
    public class RecordDocumentSerializationTests
    {
        [TestMethod]
        public void ShouldParseClassToDocument()
        {
            TestProfileClass profile = new TestProfileClass();
            profile.ORID = new ORID(8, 0);
            profile.OVersion = 1;
            profile.OClassId = 8;
            profile.Name = "Johny";
            profile.Surname = "Bravo";

            ODocument document = profile.ToDocument();

            Assert.AreEqual(document.ORID, profile.ORID);
            Assert.AreEqual(document.OVersion, profile.OVersion);
            Assert.AreEqual(document.OClassId, profile.OClassId);
            Assert.AreEqual(document.OClassName, typeof(TestProfileClass).Name);

            Assert.AreEqual(document.GetField<string>("Name"), profile.Name);
            Assert.AreEqual(document.GetField<string>("Surname"), profile.Surname);
        }

        [TestMethod]
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

            Assert.AreEqual(document.ORID, profile.ORID);
            Assert.AreEqual(document.OVersion, profile.OVersion);
            Assert.AreEqual(document.OClassId, profile.OClassId);
            Assert.AreEqual(document.OClassName, "OtherProfileClass");

            Assert.AreEqual(document.GetField<string>("Name"), profile.Name);
            Assert.AreEqual(document.GetField<string>("Surname"), profile.Surname);
        }

        class TestArray
        {
            public int[] values { get; set; }
        }

        class TestList
        {
            public List<int> values { get; set; }
        }

        [TestMethod]
        public void TestSerializeArray()
        {
            string recordString = "TestArray@values:[1,2,3,4,5]";

            ODocument document = ODocument.Deserialize(recordString);

            TypeMapper<TestArray> tm = TypeMapper<TestArray>.Instance;
            var t = new TestArray();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.values);
            Assert.AreEqual(5, t.values.Length);
            Assert.AreEqual(3, t.values[2]);

            ODocument newODocument = ODocument.ToDocument(t);
            var serialized = newODocument.Serialize();
            Assert.AreEqual(recordString, serialized);
        }

        [TestMethod]
        public void TestDeserializeList()
        {
            string recordString = "values:[1,2,3,4,5]";

            ODocument document = ODocument.Deserialize(recordString);


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

        [TestMethod]
        public void TestDeserializeSubObject()
        {
            string recordString = "TheThing:(Value:17,Text:\"blah\")";

            ODocument document = ODocument.Deserialize(recordString);


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

        [TestMethod]
        public void TestDeserializeSubObjectArray()
        {
            string recordString = "TheThings:[(Value:17,Text:\"blah\"),(Value:18,Text:\"foo\")]";

            ODocument document = ODocument.Deserialize(recordString);


            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings);
            Assert.AreEqual(2, t.TheThings.Length);
            Assert.AreEqual(18, t.TheThings[1].Value);
            Assert.AreEqual("foo", t.TheThings[1].Text);

        }

        [TestMethod]
        public void TestDeserializeSingleSubObjectArray()
        {
            string recordString = "TheThings:[(Value:18,Text:\"foo\")]";

            ODocument document = ODocument.Deserialize(recordString);


            TypeMapper<TestHasThings> tm = TypeMapper<TestHasThings>.Instance;
            var t = new TestHasThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings);
            Assert.AreEqual(1, t.TheThings.Length);
            Assert.AreEqual(18, t.TheThings[0].Value);
            Assert.AreEqual("foo", t.TheThings[0].Text);

        }

        [TestMethod]
        public void TestDeserializeEmptySubObjectArray()
        {
            string recordString = "TheThings:[]";

            ODocument document = ODocument.Deserialize(recordString);


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

        [TestMethod]
        public void TestDeserializeSubObjectList()
        {
            string recordString = "TheThings:[(Value:17,Text:\"blah\"),(Value:18,Text:\"foo\")]";

            ODocument document = ODocument.Deserialize(recordString);


            TypeMapper<TestHasListThings> tm = TypeMapper<TestHasListThings>.Instance;
            var t = new TestHasListThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings);
            Assert.AreEqual(2, t.TheThings.Count);
            Assert.AreEqual(18, t.TheThings[1].Value);
            Assert.AreEqual("foo", t.TheThings[1].Text);

        }

        [TestMethod]
        public void TestDeserializeSingleSubObjectList()
        {
            string recordString = "TheThings:[(Value:18,Text:\"foo\")]";

            ODocument document = ODocument.Deserialize(recordString);


            TypeMapper<TestHasListThings> tm = TypeMapper<TestHasListThings>.Instance;
            var t = new TestHasListThings();
            tm.ToObject(document, t);

            Assert.IsNotNull(t.TheThings);
            Assert.AreEqual(1, t.TheThings.Count);

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

        [TestMethod]
        public void TestDeserializationMapping()
        {
            // important if you use ordered edges, since if more than 1 they appear as a list, if only one then as a single object, ie
            //    db.Create.Class<Person>().Extends("V").Run();
            //    db.Command("create property Person.in_FriendOf ANY");
            //    db.Command("alter property Person.in_FriendOf custom ordered=true");

            string recordString = "single:#10:12345,list:[#11:123,#22:1234,#33:1234567],Link:#11:123,Value:17,Text:\"some text\"";

            ODocument document = ODocument.Deserialize(recordString);
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

        [TestMethod]
        public void ShouldDeserializeSingleAndSetOfOrids()
        {
            string recordString = "single:#10:12345,set:<#11:123,#22:1234,#33:1234567>,singleSet:<#44:44>";

            ODocument document = ODocument.Deserialize(recordString);


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


    }
}
