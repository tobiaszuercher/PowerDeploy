using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;

using Raven.Client.Document;

using ServiceStack.Logging;
using ServiceStack.Testing;

namespace Powerdeploy.Server.Tests
{
    [TestClass]
    public class PackageServiceTests
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
            container.RegisterAutoWired<PackageProvider>();
            container.RegisterAutoWired<PackageService>();
        }

        [TestMethod]
        public void Synchronize_Packages()
        {
            var target = _appHost.TryResolve<PackageService>();
            var response = target.Any(new SynchronizePackageRequest());


        }
    }
}