using System;
using System.IO;
using NUnit.Framework;

namespace Orient.Tests
{
    [TestFixture]
    public class AssemblySetup
    {
        [TestFixtureSetUp()]
        public static void Setup(TestContext context)
        {
            // orientDbDir needs to point to the path of an OrientDB installation (pointing to the folder that contains the bin, lib, config sub folders)
            var orientDBDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\..\orient.server"));
            // jreDir needs to point to a JRE (or JDK)
            var jreDir = @"C:\Program Files\Java\jre7";
            DbRunner.StartOrientDb(orientDBDir, jreDir);
        }

        [TestFixtureTearDown()]
        public static void TearDown()
        {
            DbRunner.StopOrientDb();
        }
    }
}
