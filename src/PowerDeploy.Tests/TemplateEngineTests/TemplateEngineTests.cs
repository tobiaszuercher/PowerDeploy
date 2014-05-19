using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

using Moq;

using PowerDeploy.Core;
using PowerDeploy.Core.Template;

namespace PowerDeploy.Tests.TemplateEngineTests
{
    [TestFixture]
    public class TemplateEngineTests
    {
        [Test]
        public void Transform_Package_Test()
        {
            var mock = new Mock<IEnvironmentSerializer>();
            mock.Setup(provider => provider.Deserialize("unit")).Returns(GetUnitEnvironment());

            var target = new TemplateEngine();
            ////target.ConfigurePackage(@"c:\temp\nuget\Testpackage.1.0.0.nupkg", "DEV", @"c:\temp\");
            /// // TODO:
        }

        [Test]
        public void Transform_Read_Only_File()
        {
            var mock = new Mock<IEnvironmentSerializer>();
            mock.Setup(provider => provider.Deserialize("unit")).Returns(GetUnitEnvironment());

            var target = new TemplateEngine();
            using (var dir = new TestFolder())
            {
                dir.AddFile("read-only.template.txt", "whatever: ${var1}");
                var file = dir.AddFile("read-only.txt", "empty");

                File.SetAttributes(Path.Combine(dir.DirectoryInfo.FullName, file), FileAttributes.ReadOnly);

                target.TransformDirectory(dir.DirectoryInfo.FullName, GetUnitEnvironment(), false);

            }
        }

        private Environment GetUnitEnvironment()
        {
            return new Environment()
            {
                Name = "Unit",
                Description = "UnitTest",
                Variables = new List<Variable>()
                {
                    new Variable() { Name = "Var1", Value = "Val1" }, 
                    new Variable() { Name = "Var2", Value = "Val2" }, 
                    new Variable() { Name = "Var3", Value = "Val3" },
                }
            };
        }
    }
}