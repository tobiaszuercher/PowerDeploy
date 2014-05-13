using System.IO;
using NUnit.Framework;
using PowerDeploy.Core;
using PowerDeploy.Server;

namespace PowerDeploy.Tests.Misc
{
    [TestFixture]
    public class GitTests
    {
        [Test]
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