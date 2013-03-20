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
            Assert.AreEqual(record.GetField("nick"), "ThePresident");

            Assert.AreEqual(record.GetField<List<object>>("follows").Count, new List<object>().Count);


            List<ORID> recordFollowers = record.GetField<List<ORID>>("followers");
            List<ORID> followers = new List<ORID> { new ORID("#10:5"), new ORID("#10:6") };

            Assert.AreEqual(recordFollowers.Count, followers.Count);
            Assert.AreEqual(recordFollowers[0], followers[0]);
            Assert.AreEqual(recordFollowers[1], followers[1]);
            
            Assert.AreEqual(record.GetField("name"), "Barack");
            Assert.AreEqual(record.GetField("surname"), "Obama");
            Assert.AreEqual(record.GetField<ORID>("location"), new ORID("#3:2"));
            Assert.AreEqual(record.GetField("invitedBy"), null);
            Assert.AreEqual(record.GetField("salary_cloned"), null);
            Assert.AreEqual(record.GetField<float>("salary"), 120.3f);
        }
    }
}
