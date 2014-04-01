using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using ServiceStack;

using System.Linq;

namespace PowerDeploy.IISDeployService.Tests.Unit
{
    [TestClass]
    public class DirectoryHelperTests
    {
        [TestMethod]
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