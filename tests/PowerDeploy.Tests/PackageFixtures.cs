using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NuGet;

using PowerDeploy.Core;

using Environment = PowerDeploy.Core.Environment;
using IFileSystem = PowerDeploy.Core.IFileSystem;
using PhysicalFileSystem = PowerDeploy.Core.PhysicalFileSystem;

namespace PowerDeploy.Tests
{
    public abstract class PackageFixtures
    {
        private string _originalDirectory;

        protected IFileSystem FileSystem { get; set; }

        [TestInitialize]
        public void InitTests()
        {
            _originalDirectory = System.Environment.CurrentDirectory;
            FileSystem = new PhysicalFileSystem();

            var root = new DirectoryInfo(new Uri(typeof(PackageFixtures).Assembly.CodeBase).LocalPath).Parent.FullName;
            
            // abuse .gitignore file to find out where the root dir is
            while (!Directory.GetFiles(root).Any(f => f.Contains(".gitignore")))
            {
                Console.WriteLine("DEBUG: " + root);
                root = Directory.GetParent(root).FullName;
            }

            System.Environment.CurrentDirectory = Path.Combine(root, "Samples");
        }

        protected static void MsBuild(string commandLineArguments)
        {
            var netFx = System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory();
            var msBuild = Path.Combine(netFx, "msbuild.exe");
            if (!File.Exists(msBuild))
            {
                Assert.Fail("Could not find MSBuild at: " + msBuild);
            }

            var allOutput = new StringBuilder();

            Action<string> writer = (output) =>
            {
                allOutput.AppendLine(output);
                Trace.WriteLine(output);
            };

            var result = SilentProcessRunner.ExecuteCommand(msBuild, commandLineArguments, System.Environment.CurrentDirectory, writer, e => writer("ERROR: " + e));

            if (result != 0)
            {
                Assert.Fail("MSBuild returned a non-zero exit code: " + result);
            }
        }

        // todo: read from dir
        protected Environment GetUnitEnvironment()
        {
            return new Environment()
            {
                Name = "Unit",
                Description = "UnitTest",
                Variables = new List<Variable>()
                {
                    new Variable() { Name = "xcopy.unit.variable1", Value = "Val1" }, 
                    new Variable() { Name = "xcopy.unit.variable2", Value = "Val2" }, 
                    new Variable() { Name = "SampleAppConsole_Destination", Value = @"c:\temp" },
                    new Variable() { Name = "env", Value = "UNIT" }
                }
            };
        }

        protected void Clean(string directory)
        {
            new PhysicalFileSystem().DeleteDirectory(Path.Combine(System.Environment.CurrentDirectory, directory));
        }

        protected static void AssertPackage(string packageFilePath, Action<ZipPackage> packageAssertions)
        {
            var fullPath = Path.Combine(System.Environment.CurrentDirectory, packageFilePath);
            if (!File.Exists(fullPath))
            {
                Assert.Fail("Could not find package file: " + fullPath);
            }

            Trace.WriteLine("Checking package: " + fullPath);
            var package = new ZipPackage(fullPath);
            packageAssertions(package);

            Trace.WriteLine("Success!");
        }

        [TestCleanup]
        public void CleanupTest()
        {
            System.Environment.CurrentDirectory = _originalDirectory;
            FileSystem.DeleteTempWorkingDirs();
        }
    }
}