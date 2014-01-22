using System.Collections.Generic;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using NuGet;

using PowerDeploy.Core;
using PowerDeploy.Core.Deploy;
using PowerDeploy.Core.Extensions;

using PackageManager = PowerDeploy.Core.PackageManager;

namespace PowerDeploy.Tests.Integration
{
    [TestClass]
    public class XCopyTests : PackageFixtures
    {
        [TestMethod]
        public void Build_And_Package_XCopy_Sample_Project_Test()
        {
            MsBuild("PowerDeploy.Sample.XCopy\\PowerDeploy.Sample.XCopy.csproj /t:clean,build /p:RunOctoPack=true /p:OctoPackPackageVersion=1.3.3.7 /p:Configuration=Release /v:m");

            Assert.IsTrue(File.Exists(@"PowerDeploy.Sample.XCopy\obj\octopacked\PowerDeploy.Sample.XCopy.1.3.3.7.nupkg"));

            var nupkg = new ZipPackage(@"PowerDeploy.Sample.XCopy\obj\octopacked\PowerDeploy.Sample.XCopy.1.3.3.7.nupkg");

            Assert.AreEqual("1.3.3.7", nupkg.Version.ToString());
            Assert.AreEqual("PowerDeploy.Sample.XCopy", nupkg.Id);
            Assert.IsTrue(nupkg.GetFiles().Any(f => f.Path.Contains("powerdeploy.template.xml")));
        }

        [TestMethod]
        public void Configure_XCopy_Sample_Project_With_Mocked_Environment_Test()
        {
            MsBuild("PowerDeploy.Sample.XCopy\\PowerDeploy.Sample.XCopy.csproj /t:clean,build /p:RunOctoPack=true /p:OctoPackPackageVersion=1.3.3.7 /p:Configuration=Release /v:m");

            var environmentMock = new Mock<IEnvironmentParser>();
            environmentMock.Setup(prov => prov.GetEnvironment(It.IsAny<string>())).Returns(GetUnitEnvironment);

            var outputPath = FileSystem.CreateTempWorkingDir();

            var target = new PackageManager(environmentMock.Object);
            target.ConfigurePackageByEnvironment(@"PowerDeploy.Sample.XCopy\obj\octopacked\PowerDeploy.Sample.XCopy.1.3.3.7.nupkg", "Unit", outputPath);

            // check if all template files are parsed
            var nupkg = new ZipPackage(Path.Combine(outputPath, "PowerDeploy.Sample.XCopy_v1.3.3.7_UNIT.nupkg"));
            Assert.IsFalse(nupkg.GetFiles().Any(f => f.Path.Contains(".template.")));

            AssertPackage(Path.Combine(outputPath, "PowerDeploy.Sample.XCopy_v1.3.3.7_UNIT.nupkg"),
                pkg => pkg.AssertFileContent(new Dictionary<string, string>()
                {
                    {
                        "powerdeploy.xml", 
                        @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                          <package type=""xcopy"" environment=""UNIT"">
                              <destination>c:\temp</destination>
                          </package>".ToXmlOneLine()
                    },
                    {
                        "App.config",
                        @"<?xml version=""1.0"" encoding=""utf-8"" ?>
                            <appSettings>
                              <add key=""variable1"" value=""Val1"" />
                              <add key=""variable2"" value=""Val2"" />
                              <add key=""default.variable"" value=""defaultvalue"" />
                            </appSettings>".ToXmlOneLine()
                    }
                }));
        }

        [TestMethod]
        public void Deserialize_XCopy_Package_Descriptor_Test()
        {
            MsBuild("PowerDeploy.Sample.XCopy\\PowerDeploy.Sample.XCopy.csproj /t:clean,build /p:RunOctoPack=true /p:OctoPackPackageVersion=1.3.3.7 /p:Configuration=Release /v:m");

            var environmentMock = new Mock<IEnvironmentParser>();
            environmentMock.Setup(prov => prov.GetEnvironment(It.IsAny<string>())).Returns(GetUnitEnvironment);

            var outputPath = FileSystem.CreateTempWorkingDir();

            var target = new PackageManager(environmentMock.Object);
            target.ConfigurePackageByEnvironment(@"PowerDeploy.Sample.XCopy\obj\octopacked\PowerDeploy.Sample.XCopy.1.3.3.7.nupkg", "Unit", outputPath);

            var nupkg = new ZipPackage(Path.Combine(outputPath, "PowerDeploy.Sample.XCopy_v1.3.3.7_UNIT.nupkg"));
            var options = nupkg.GetPackageOptions<XCopyOptions>();

            Assert.AreEqual("xcopy", nupkg.PeekPackageType());
            Assert.AreEqual(@"c:\temp", options.Destination);
            Assert.AreEqual("UNIT", options.Environment);
        }

        [TestMethod]
        public void Deploy_XCopy_Package_Test()
        {
            MsBuild("PowerDeploy.Sample.XCopy\\PowerDeploy.Sample.XCopy.csproj /t:clean,build /p:RunOctoPack=true /p:OctoPackPackageVersion=1.3.3.7 /p:Configuration=Release /v:m");

            var environmentMock = new Mock<IEnvironmentParser>();
            environmentMock.Setup(prov => prov.GetEnvironment(It.IsAny<string>())).Returns(GetUnitEnvironment);

            var outputPath = FileSystem.CreateTempWorkingDir();

            var target = new PackageManager(environmentMock.Object);
            target.ConfigurePackageByEnvironment(@"PowerDeploy.Sample.XCopy\obj\octopacked\PowerDeploy.Sample.XCopy.1.3.3.7.nupkg", "Unit", outputPath);

            target.DeployPackage(Path.Combine(outputPath, "PowerDeploy.Sample.XCopy_v1.3.3.7_UNIT.nupkg"));
        }
    }
}