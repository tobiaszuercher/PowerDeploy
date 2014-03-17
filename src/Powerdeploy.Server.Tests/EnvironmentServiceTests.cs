using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;

using Raven.Client;
using Raven.Client.Document;

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
        }

        [Ignore]
        [TestMethod]
        public void Foo()
        {
            var ds = _appHost.TryResolve<IDocumentStore>();

            using (var session = ds.OpenSession())
            {
                var e1 = new Environment() { Id = 1, Name = "DEV", Description = "Development environment for the Dev's" };
                var e2 = new Environment() { Id = 2, Name = "TEST", Description = "Development environment for the Dev's" };
                var e3 = new Environment() { Id = 3, Name = "PROD", Description = "Development environment for the Dev's" };

                session.Store(e1);
                session.Store(e2);
                session.Store(e3);

                session.SaveChanges();
            }
        }
    }
}