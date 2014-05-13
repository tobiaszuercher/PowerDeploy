using NUnit.Framework;
using PowerDeploy.IISDeployService.Contract;

using ServiceStack.Logging;
using ServiceStack.Testing;
using ServiceStack.Text;

namespace PowerDeploy.IISDeployService.Tests.Unit
{
    [TestFixture]
    public class HelloServiceTests
    {
        private BasicAppHost _appHost;

        [TestFixtureSetUp]
        public void TestInit()
        {
            LogManager.LogFactory = new ConsoleLogFactory();

            _appHost = new BasicAppHost();
            _appHost.Init();

            var container = _appHost.Container;

            container.RegisterAutoWired<HelloService>();
        }

        [TestFixtureTearDown]
        public void TestCleanup()
        {
            _appHost.Dispose();
        }

        [Test]
        [Category("Unit")]
        public void SampleTest()
        {
            var target = _appHost.Container.Resolve<HelloService>();

            var response = target.Get(new Hello() { Name = "Tobi" });
            response.PrintDump();

            Assert.AreEqual("Hello Tobi!", response);
        }
    }
}