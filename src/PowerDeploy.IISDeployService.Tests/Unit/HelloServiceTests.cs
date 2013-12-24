using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.IISDeployService.Contract;

using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.ServiceInterface.Testing;

using ServiceStack.Text;

namespace PowerDeploy.IISDeployService.Tests.Unit
{
    [TestClass]
    public class HelloServiceTests
    {
        private BasicAppHost _appHost;

        [TestInitialize]
        public void TestInit()
        {
            LogManager.LogFactory = new ConsoleLogFactory();

            _appHost = new BasicAppHost();
            _appHost.Init();

            var container = _appHost.Container;

            container.RegisterAutoWired<HelloService>();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _appHost.Dispose();
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void SampleTest()
        {
            var target = _appHost.Container.Resolve<HelloService>();

            var response = target.Get(new Hello() { Name = "Tobi" });
            response.PrintDump();

            Assert.AreEqual("Hello Tobi!", response);
        }
    }
}