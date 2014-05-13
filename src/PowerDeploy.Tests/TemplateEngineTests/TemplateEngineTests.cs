using System.Collections.Generic;

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