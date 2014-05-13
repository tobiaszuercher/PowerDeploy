using NUnit.Framework;
using PowerDeploy.Server;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;
using Raven.Tests.Helpers;
using ServiceStack.Testing;
using ConsoleLogFactory = PowerDeploy.Core.Logging.ConsoleLogFactory;
using LogManager = PowerDeploy.Core.Logging.LogManager;

namespace Powerdeploy.Server.Tests
{
    [TestFixture]
    public class ConfigurationServiceTests : RavenTestBase
    {
        private BasicAppHost _appHost;

        [TestFixtureSetUp]
        public void TestInit()
        {
            if (_appHost == null)
            {
                LogManager.LogFactory = new ConsoleLogFactory();

                _appHost = new BasicAppHost();
                _appHost.Init();

                var store = NewDocumentStore();
                DataInitializer.InitializerWithDefaultValuesIfEmpty(store);

                _appHost.Container.RegisterAutoWired<ConfigurationService>();
            }
        }

        [Test]
        [Ignore]
        public void ConfigurationService_Returns_Valid_Configration()
        {
            var target = _appHost.TryResolve<ConfigurationService>();

            var result = target.Get(new QueryServerSettings());

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.IsTrue(result.GitExecutable.Contains("git.exe"));
        }
    }
}
