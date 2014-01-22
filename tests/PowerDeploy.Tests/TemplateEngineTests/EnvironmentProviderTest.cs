using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Core;

using Environment = System.Environment;

namespace PowerDeploy.Tests.TemplateEngineTests
{
    [TestClass]
    public class EnvironmentProviderTest
    {
        [TestMethod]
        public void Find_Environment()
        {
            var target = new EnvironmentProvider();
            var result = target.GetEnvironment(@"C:\git\PowerDeploy\src\PowerDeploy.PackageManagerExtension", "unittest");

            Assert.IsNotNull(result);
            Assert.AreEqual("unittest", result.Name);
            Assert.AreEqual("Jack", result["Firstname"].Value);
            Assert.AreEqual("Bauer", result["Lastname"].Value);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Find_Environment_Not_Existing()
        {
            using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
            {
                workDir.AddFolder("dir1");
                workDir.AddFolder("dir1/subdir1");

                var target = new EnvironmentProvider();
                target.GetEnvironment(Path.Combine(workDir.DirectoryInfo.FullName, "dir1/subdir1"), "unittest");
            }
        }
    }
}