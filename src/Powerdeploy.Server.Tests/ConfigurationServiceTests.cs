using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;

using Raven.Client.Document;

using ServiceStack.Logging;
using ServiceStack.Testing;

namespace Powerdeploy.Server.Tests
{
    [TestClass]
    public class ConfigurationServiceTests
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

            container.Register(documentStore);
            container.RegisterAutoWired<ConfigurationService>();
        }

        [TestMethod]
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
