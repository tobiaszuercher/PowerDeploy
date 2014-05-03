using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Core;
using PowerDeploy.Server;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;
using PowerDeploy.Server.Tests;

using Raven.Client.Document;

using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Testing;

namespace Powerdeploy.Server.Tests
{
    [TestClass]
    public class DeployServiceTests : PackageFixtures
    {
        private BasicAppHost _appHost;

        [TestInitialize]
        public void TestInit()
        {
            LogManager.LogFactory = new ConsoleLogFactory();

            _appHost = new BasicAppHost();
            _appHost.Init();

            var container = _appHost.Container;

            var documentStore = new DocumentStore()
            {
                DefaultDatabase = "PowerDeploy",
                Url = "http://localhost:8080",
            }.Initialize();

            Bootstrapper.ConfigureDependencies(container, documentStore);
        }

        [TestMethod]
        public void Synchronize_Packages()
        {
            var target = _appHost.TryResolve<PackageService>();
            var response = target.Any(new SynchronizePackageRequest());
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Deploy_XCopy_Package()
        {
            // arrange
            const string Version = "1.3.3.7";
            const string Package = "PowerDeploy.Sample.XCopy";
            const string TargetEnvironment = "unittest";

            var target = _appHost.Resolve<DeployService>();

            // make sure nuget server has no package with this version
            File.Delete(Path.Combine(NugetServerPackagesPath, "{0}.{1}.nupkg".Fmt(Package, Version)));

            MsBuild(@"{0}\{0}.csproj /t:clean,build /p:OctoPackPackageVersion={1} /p:OctoPackPublishPackageToFileShare={2} /p:Configuration=Release /p:RunOctoPack=true /v:m".Fmt(Package, Version, NugetServerPackagesPath));
            
            // act
            target.Post(new TriggerDeployment()
            {
                Environment = TargetEnvironment, 
                PackageId = Package,
                Version = Version,
            });

            // assert
            var environment = new XmlEnvironmentSerializer().Deserialize(@".powerdeploy\{0}.xml".Fmt(TargetEnvironment));
            var targetPath = environment["SampleAppConsole_Destination"];

            Assert.IsTrue(File.Exists(Path.Combine(targetPath.Value, "PowerDeploy.SampleApp.ConsoleXCopy.exe")));
        }
    }
}