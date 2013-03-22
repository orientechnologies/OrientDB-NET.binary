using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests.Serialization
{
    [TestClass]
    public class RecordSerializationTests
    {
        [TestMethod]
        public void ShouldSerializeNull()
        {
            string recordString = "TestClass@null:,embedded:(null:)";

            ORecord record = new ORecord();
            record.ClassName = "TestClass";
            record.SetField<object>("null", null);
            record.SetField<object>("embedded.null", null);

            string serializedRecord = record.Serialize();

            Assert.AreEqual(serializedRecord, recordString);
        }

        [TestMethod]
        public void ShouldSerializeBoolean()
        {
            string recordString = "TestClass@isTrue:true,isFalse:false,embedded:(isTrue:true,isFalse:false),array:[true,false]";

            ORecord record = new ORecord();
            record.ClassName = "TestClass";
            record.SetField<bool>("isTrue", true);
            record.SetField<bool>("isFalse", false);
            record.SetField<bool>("embedded.isTrue", true);
            record.SetField<bool>("embedded.isFalse", false);
            record.SetField<List<bool>>("array", new List<bool> { true, false });

            string serializedRecord = record.Serialize();

            Assert.AreEqual(serializedRecord, recordString);
        }
    }
}
