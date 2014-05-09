using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerDeploy.Core;
using PowerDeploy.Server;

namespace Powerdeploy.Server.Tests
{
    [TestClass]
    public class GitTests
    {
        [TestMethod]
        public void Pull()
        {
            var filesystem = new PhysicalFileSystem();
            var workDir = filesystem.CreateTempWorkingDir();

            var target = new GitCommandLineWrapper(workDir);
            target.Init();

            Assert.IsTrue(filesystem.DirectoryExists(Path.Combine(workDir, ".git")));

            filesystem.DeleteTempWorkingDirs();
        }
    }
}