using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orient.Client;

namespace Orient.Tests
{
    [TestClass]
    public class TestConfigurationOperation
    {
        [TestMethod]
        public void TestConfigGet()
        {
            OServer server = TestConnection.GetServer();
            bool IsCreated = server.ConfigSet("network.retry", "6");
            string value = server.ConfigGet("network.retry");
            Assert.AreEqual("6", value);
        }
        [TestMethod]
        public void TestConfigList()
        {
            OServer server = TestConnection.GetServer();
            Dictionary<string, string> config = server.ConfigList();
            Assert.IsTrue(config.Count > 0);
        }
        [TestMethod]
        public void TestConfigSet()
        {
            OServer server = TestConnection.GetServer();
            // Only Set existing keys
            // Don't create new one
            bool IsCreated = server.ConfigSet("network.retry", "6");
            string loadedValue = server.ConfigGet("network.retry");
            Assert.IsTrue(IsCreated);
            Assert.AreEqual("6", loadedValue);
        }
    }
}
