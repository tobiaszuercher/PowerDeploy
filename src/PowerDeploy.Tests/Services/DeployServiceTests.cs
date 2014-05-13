using System.IO;
using NUnit.Framework;
using PowerDeploy.Core;
using PowerDeploy.Core.Logging;
using PowerDeploy.Server;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;
using Raven.Client;
using ServiceStack;
using ServiceStack.Testing;

namespace PowerDeploy.Tests.Services
{
    [TestFixture]
    public class DeployServiceTests : PackageFixtures
    {
        private BasicAppHost _appHost;

        ////[TestFixtureSetUp]
        ////public void TestInit()
        ////{
        ////    if (_appHost == null)
        ////    {
        ////        LogManager.LogFactory = new ConsoleLogFactory();

        ////        _appHost = new BasicAppHost();
        ////        _appHost.Init();

        ////        var store = NewDocumentStore();
        ////        DataInitializer.InitializerWithDefaultValuesIfEmpty(store);

        ////        Bootstrapper.ConfigureDependencies(_appHost.Container, store);
        ////    }
        ////}

        [Test]
        [Category("Integration")]
        [Ignore] // todo: Think about how to test nuget server
        public void Deploy_XCopy_Package()
        {
            // arrange
            const string Version = "1.3.3.7";
            const string Package = "PowerDeploy.Sample.XCopy";
            const string TargetEnvironment = "unittest";

            _appHost.Container.Resolve<IDocumentStore>().CreateSzenario()
                .PublishPackage(Package, Version)
                .Play();

            System.Environment.CurrentDirectory = TestBuddy.GetProjectRootCombined("samples");

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
