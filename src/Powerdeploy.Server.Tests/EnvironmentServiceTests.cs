using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;

using Raven.Client;
using Raven.Client.Document;

using ServiceStack;
using ServiceStack.Logging;
using ServiceStack.Testing;

namespace Powerdeploy.Server.Tests
{
    [TestClass]
    public class EnvironmentServiceTests
    {
        private BasicAppHost _appHost;

        [TestInitialize]
        public void TestInit()
        {
            LogManager.LogFactory = new ConsoleLogFactory();

            _appHost = new BasicAppHost();
            _appHost.Init();

            var container = _appHost.Container;

            var documentStore = new DocumentStore() { DefaultDatabase = "PowerDeploy", Url = "http://localhost:8080" }.Initialize();

            container.Register(documentStore);
            container.RegisterAutoWired<EnvironmentService>();

            var bla = new JsonServiceClient("http://localhost");
            bla.Get(new QueryEnvironment() { Id = 1 });
        }

        [Ignore]
        [TestMethod]
        public void Foo()
        {
            var ds = _appHost.TryResolve<IDocumentStore>();
        }
    }
}