using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using NuGet;

using PowerDeploy.Core;

using Environment = System.Environment;
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
            _originalDirectory = Environment.CurrentDirectory;
            FileSystem = new PhysicalFileSystem();

            var root = new Uri(typeof(PackageFixtures).Assembly.CodeBase).LocalPath;
            while (!root.EndsWith("PowerDeploy") && !root.EndsWith("PowerDeploy\\"))
            {
                root = Path.GetFullPath(Path.Combine(root, @".."));
            }

            Environment.CurrentDirectory = Path.Combine(root, "Samples");

            // do that with /t:clean on msbuild because cleaning isn't that easy (no access to file etc)
            ////Clean(@"PowerDeploy.Sample.XCopy\bin");
            ////Clean(@"PowerDeploy.Sample.XCopy\obj");
            ////Clean(@"PowerDeploy.Sample.WebApp\bin");
            ////Clean(@"PowerDeploy.Sample.WebApp\obj");
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

            var result = SilentProcessRunner.ExecuteCommand(msBuild, commandLineArguments, Environment.CurrentDirectory, writer, e => writer("ERROR: " + e));

            if (result != 0)
            {
                Assert.Fail("MSBuild returned a non-zero exit code: " + result);
            }
        }

        protected void Clean(string directory)
        {
            new PhysicalFileSystem().DeleteDirectory(Path.Combine(Environment.CurrentDirectory, directory));
        }

        protected static void AssertPackage(string packageFilePath, Action<ZipPackage> packageAssertions)
        {
            var fullPath = Path.Combine(Environment.CurrentDirectory, packageFilePath);
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
            Environment.CurrentDirectory = _originalDirectory;
            FileSystem.DeleteTempWorkingDirs();
        }
    }
}