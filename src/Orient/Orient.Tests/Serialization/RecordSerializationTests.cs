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
            record.SetField("isTrue", true);
            record.SetField("isFalse", false);
            record.SetField("embedded.isTrue", true);
            record.SetField("embedded.isFalse", false);
            record.SetField<List<bool>>("array", new List<bool> { true, false });

            string serializedRecord = record.Serialize();

            Assert.AreEqual(serializedRecord, recordString);
        }

        [TestMethod]
        public void ShouldSerializeNumbers()
        {
            string recordString = "TestClass@ByteNumber:123b,ShortNumber:1234s,IntNumber:123456,LongNumber:12345678901l,FloatNumber:3.14f,DoubleNumber:3.14d,DecimalNumber:1234567.8901c,embedded:(ByteNumber:123b,ShortNumber:1234s,IntNumber:123456,LongNumber:12345678901l,FloatNumber:3.14f,DoubleNumber:3.14d,DecimalNumber:1234567.8901c)";

            ORecord record = new ORecord();
            record.ClassName = "TestClass";
            record.SetField("ByteNumber", byte.Parse("123"));
            record.SetField("ShortNumber", short.Parse("1234"));
            record.SetField("IntNumber", 123456);
            record.SetField("LongNumber", 12345678901);
            record.SetField("FloatNumber", 3.14f);
            record.SetField("DoubleNumber", 3.14);
            record.SetField("DecimalNumber", new Decimal(1234567.8901));
            record.SetField("embedded.ByteNumber", byte.Parse("123"));
            record.SetField("embedded.ShortNumber", short.Parse("1234"));
            record.SetField("embedded.IntNumber", 123456);
            record.SetField("embedded.LongNumber", 12345678901);
            record.SetField("embedded.FloatNumber", 3.14f);
            record.SetField("embedded.DoubleNumber", 3.14);
            record.SetField("embedded.DecimalNumber", new Decimal(1234567.8901));

            string serializedRecord = record.Serialize();

            Assert.AreEqual(serializedRecord, recordString);
        }

        [TestMethod]
        public void ShouldSerializeDateTime()
        {
            DateTime dateTime = DateTime.Now;

            // get Unix time version
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            string timeString = ((long)((DateTime)dateTime - unixEpoch).TotalMilliseconds).ToString();

            string recordString = "TestClass@DateTime:" + timeString + "t,embedded:(DateTime:" + timeString + "t)";

            ORecord record = new ORecord();
            record.ClassName = "TestClass";
            record.SetField("DateTime", dateTime);
            record.SetField("embedded.DateTime", dateTime);

            string serializedRecord = record.Serialize();

            Assert.AreEqual(serializedRecord, recordString);
        }

        [TestMethod]
        public void ShouldSerializeStrings()
        {
            string recordString = "TestClass@String:\"Bra\\" + "\"vo \\\\ asdf\",Array:[\"foo\",\"bar\"],embedded:(String:\"Bra\\" + "\"vo \\\\ asdf\",Array:[\"foo\",\"bar\"])";

            ORecord record = new ORecord();
            record.ClassName = "TestClass";
            record.SetField("String", "Bra\"vo \\ asdf");
            record.SetField("Array", new List<string> { "foo", "bar" });
            record.SetField("embedded.String", "Bra\"vo \\ asdf");
            record.SetField("embedded.Array", new List<string> { "foo", "bar" });

            string serializedString = record.Serialize();

            Assert.AreEqual(serializedString, recordString);
        }

        [TestMethod]
        public void ShouldSerializeORIDs()
        {
            string recordString = "TestClass@Single:#8:0,Array:[#8:1,#8:2],embedded:(Single:#9:0,Array:[#9:1,#9:2])";

            ORecord record = new ORecord();
            record.ClassName = "TestClass";
            record.SetField("Single", new ORID(8, 0));
            record.SetField("Array", new List<ORID> { new ORID(8, 1), new ORID(8, 2) });
            record.SetField("embedded.Single", new ORID(9, 0));
            record.SetField("embedded.Array", new List<ORID> { new ORID(9, 1), new ORID(9, 2) });

            string serializedString = record.Serialize();

            Assert.AreEqual(serializedString, recordString);
        }
    }
}
