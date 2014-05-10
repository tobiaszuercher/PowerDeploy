using Microsoft.VisualStudio.TestTools.UnitTesting;
using PowerDeploy.Server;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;
using Raven.Tests.Helpers;
using ServiceStack.Testing;
using ConsoleLogFactory = PowerDeploy.Core.Logging.ConsoleLogFactory;
using LogManager = PowerDeploy.Core.Logging.LogManager;

namespace Powerdeploy.Server.Tests
{
    [TestClass]
    public class ConfigurationServiceTests : RavenTestBase
    {
        private BasicAppHost _appHost;

        [TestInitialize]
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

        [TestMethod]
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
