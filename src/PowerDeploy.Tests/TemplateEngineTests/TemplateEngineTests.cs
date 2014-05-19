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
                dir.AddFile("read-only.txt", "will be transformed").SetReadOnly();

                // before the bugfix: this threw a Exception because the file was ReadOnly
                target.TransformDirectory(dir.DirectoryInfo.FullName, GetUnitEnvironment(), false);

                Assert.AreNotEqual("will be transformed", dir.ReadFile("read-only.txt"));
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
                    new Variable() { Name = "var1", Value = "Val1" }, 
                    new Variable() { Name = "var2", Value = "Val2" }, 
                    new Variable() { Name = "ar3", Value = "Val3" },
                }
            };
        }
    }
}