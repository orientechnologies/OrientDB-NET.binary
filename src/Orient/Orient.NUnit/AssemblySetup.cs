using System;
using System.IO;
using NUnit.Framework;

namespace Orient.Tests
{
    [SetUpFixture]
    public class AssemblySetup
    {
        private static string _orientServerDirectory = @"..\..\..\..\..\..\orient.server";

        [SetUp]
        public static void Setup()
        {
            // orientDbDir needs to point to the path of an OrientDB installation (pointing to the folder that contains the bin, lib, config sub folders)
            var orientDBDir = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, _orientServerDirectory));
            // jreDir needs to point to a JRE (or JDK)
            var jreDir = @"C:\Program Files\Java\jre7";
            DbRunner.StartOrientDb(orientDBDir, jreDir);
        }

        [TearDown]
        public static void TearDown()
        {
            DbRunner.StopOrientDb(_orientServerDirectory);
        }
    }
}
