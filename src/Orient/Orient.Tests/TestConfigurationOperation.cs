using System;
using System.Collections.Generic;
using Xunit;
using Orient.Client;

namespace Orient.Tests
{
    
    public class TestConfigurationOperation
    {
        [Fact]
        public void TestConfigGet()
        {
            OServer server = TestConnection.GetServer();
            bool IsCreated = server.ConfigSet("network.retry", "6");
            string value = server.ConfigGet("network.retry");
            Assert.Equal("6", value);
        }
        [Fact]
        public void TestConfigList()
        {
            OServer server = TestConnection.GetServer();
            Dictionary<string, string> config = server.ConfigList();
            Assert.True(config.Count > 0);
        }
        [Fact]
        public void TestConfigSet()
        {
            OServer server = TestConnection.GetServer();
            // Only Set existing keys
            // Don't create new one
            bool IsCreated = server.ConfigSet("network.retry", "6");
            string loadedValue = server.ConfigGet("network.retry");
            Assert.True(IsCreated);
            Assert.Equal("6", loadedValue);
        }
    }
}
