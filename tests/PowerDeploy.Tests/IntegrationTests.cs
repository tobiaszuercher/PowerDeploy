using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using NuGet;

using PowerDeploy.Core;
using PowerDeploy.Core.Template;

using PackageManager = PowerDeploy.Core.PackageManager;

namespace PowerDeploy.Tests
{
    [TestClass]
    public class IntegrationTests : PackageFixtures
    {
        [TestMethod]
        public void Foo()
        {
            MsBuild("PowerDeploy.SampleApp.ConsoleXCopy\\PowerDeploy.SampleApp.ConsoleXCopy.csproj /p:RunOctoPack=true /p:OctoPackPackageVersion=1.0.0.1 /p:Configuration=Release /v:m");

            var environmentMock = new Mock<IEnviornmentProvider>();
            environmentMock.Setup(prov => prov.GetVariables(It.IsAny<string>())).Returns(GetUnitEnvironment);

            var target = new PackageManager(environmentMock.Object);
            target.ConfigurePackage(@"PowerDeploy.SampleApp.ConsoleXCopy\obj\octopacked\PowerDeploy.SampleApp.ConsoleXCopy.1.0.0.1.nupkg", "unit", @"c:\temp");

            var nupkg = new ZipPackage(@"PowerDeploy.SampleApp.ConsoleXCopy\obj\octopacked\PowerDeploy.SampleApp.ConsoleXCopy.1.0.0.1.nupkg");
            var bla = nupkg.GetFiles().ToList();
        }

        // todo: read from dir
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
                    new Variable() { Name = "SampleAppConsole_Destination", Value = "c:\temp" },
                }
            };
        }
    }
}