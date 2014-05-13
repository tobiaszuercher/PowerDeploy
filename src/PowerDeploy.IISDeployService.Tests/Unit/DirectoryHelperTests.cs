using System;
using System.IO;

using NUnit.Framework;

using ServiceStack;

using System.Linq;

namespace PowerDeploy.IISDeployService.Tests.Unit
{
    [TestFixture]
    public class DirectoryHelperTests
    {
        [Test]
        public void Cleanup_Folder_with_less_than_5_Folders()
        {
            var target = Directory.CreateDirectory(@"c:\temp\{0}".Fmt(Guid.NewGuid()));

            Directory.CreateDirectory(Path.Combine(target.FullName, "App_v1.0.0"));
            Directory.CreateDirectory(Path.Combine(target.FullName, "App_v1.0.1"));
            Directory.CreateDirectory(Path.Combine(target.FullName, "App_v1.0.2"));

            DirectoryHelper.DeleteOldFolders(target.FullName, "app", 5);

            target.Refresh();

            Assert.AreEqual(3, target.GetDirectories().Count());
        }
    }
}