using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using PowerDeploy.Core;
using PowerDeploy.Core.Template;
using Xunit;
using Assert = NUnit.Framework.Assert;
using Environment = System.Environment;

namespace PowerDeploy.Tests.TemplateEngineTests
{
    public class EnvironmentProviderTest
    {
        [Test]
        public void Find_Environment()
        {
            var target = new EnvironmentProvider();

            target.Initialize(TestBuddy.GetProjectRootCombined(@"samples\PowerDeploy.Sample.XCopy"));
            var result = target.GetEnvironment("unittest");

            Assert.IsNotNull(result);
            Assert.AreEqual("unittest", result.Name);
            Assert.AreEqual("Jack", result["Firstname"].Value);
            Assert.AreEqual("Bauer", result["Lastname"].Value);
        }

        [Test]
        [NUnit.Framework.ExpectedException(typeof(DirectoryNotFoundException))]
        public void Find_Environment_With_Dir_Not_Existing()
        {
            using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
            {
                workDir.AddFolder("dir1");
                workDir.AddFolder("dir1/subdir1");

                var target = new EnvironmentProvider(Path.Combine(workDir.DirectoryInfo.FullName, "dir1/subdir1"));
                target.GetEnvironment("unittest");
            }
        }

        [Test]
        [NUnit.Framework.ExpectedException(typeof(FileNotFoundException))]
        public void Find_Non_Existing_Environment()
        {
            using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
            {
                workDir.AddFolder("dir1");
                workDir.AddFolder(".powerdeploy");
                workDir.AddFolder("dir1/subdir1");

                var target = new EnvironmentProvider(Path.Combine(workDir.DirectoryInfo.FullName, "dir1/subdir1"));
                target.GetEnvironment("unittest");
            }
        }

        [Test]
        public void Find_Environment_In_Same_Dir()
        {
            var xml = @"<?xml version=""1.0""?>
                        <environment name=""local"" description=""Used for unit tests, not a real environment"">
                          <variable name=""Name"" value=""Tobi"" />
                          <variable name=""Jack"" value=""Bauer"" />
                        </environment>";

            using (var workDir = new TestFolder(Environment.SpecialFolder.LocalApplicationData))
            {
                workDir.AddFolder("dir1");
                workDir.AddFolder("dir1/subdir1");
                workDir.AddFolder(".powerdeploy");
                workDir.AddFile(".powerdeploy/unittest.xml", xml);

                var target = new EnvironmentProvider(workDir.DirectoryInfo.FullName);
                var result = target.GetEnvironment("unittest");

                Assert.AreEqual("local", result.Name);
                Assert.AreEqual("Tobi", result["Name"].Value);
                Assert.AreEqual("Bauer", result["Jack"].Value);
            }
        }
    }
}