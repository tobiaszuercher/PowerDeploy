using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Core;

using Environment = System.Environment;

namespace PowerDeploy.Tests
{
    public abstract class PackageFixtures
    {
        private string _originalDirectory;

        [TestInitialize]
        public void InitTests()
        {
            _originalDirectory = Environment.CurrentDirectory;

            var root = new Uri(typeof(PackageFixtures).Assembly.CodeBase).LocalPath;
            while (!root.EndsWith("PowerDeploy.Dashboard") && !root.EndsWith("PowerDeploy.Dashboard\\"))
            {
                root = Path.GetFullPath(Path.Combine(root, "..\\"));
            }

            Environment.CurrentDirectory = root;

            Clean(@"PowerDeploy.SampleApp.ConsoleXCopy\bin");
            Clean(@"PowerDeploy.SampleApp.ConsoleXCopy\obj");
            Clean(@"PowerDeploy.SampleApp.SampleWebApp\bin");
            Clean(@"PowerDeploy.SampleApp.SampleWebApp\obj");
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

        [TestCleanup]
        public void CleanupTest()
        {
            Environment.CurrentDirectory = _originalDirectory;
        }
    }
}