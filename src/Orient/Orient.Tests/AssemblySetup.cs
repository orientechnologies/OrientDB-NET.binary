using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Orient.Tests
{
    [TestClass]
    public class AssemblySetup
    {
        [AssemblyInitialize()]
        public static void Setup(TestContext context)
        {
            // orientDbDir needs to point to the path of an OrientDB installation (pointing to the folder that contains the bin, lib, config sub folders)
            var orientDBDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\..\..\orient.server"));
            // jreDir needs to point to a JRE (or JDK)
            var jreDir = @"C:\Program Files\Java\jre7";
            DbRunner.StartOrientDb(orientDBDir, jreDir);
        }

        [AssemblyCleanup()]
        public static void TearDown()
        {
            DbRunner.StopOrientDb();
        }
    }
}