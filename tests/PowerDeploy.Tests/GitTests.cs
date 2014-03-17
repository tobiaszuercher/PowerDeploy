using System;
using System.Diagnostics;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Core;

namespace PowerDeploy.Tests
{
    [TestClass]
    public class GitTests
    {
        [TestMethod]
        public void Pull()
        {
            Action<string> writer = (output) => Trace.WriteLine(output);

            SilentProcessRunner.ExecuteCommand("git", "pull -q", @"c:\git\ServiceStack", writer, e => writer("ERROR: " + e));
        }
    }
}