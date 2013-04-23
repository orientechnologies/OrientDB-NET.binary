using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

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
    }
}
