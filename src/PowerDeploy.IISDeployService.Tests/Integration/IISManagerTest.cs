using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.IISDeployService.Contract;

namespace PowerDeploy.IISDeployService.Tests.Integration
{
    [TestClass]
    public class IISManagerTest
    {
        [TestMethod]
        [TestCategory("Integration")]
        public void Create_AppPool_Test()
        {
            IISManagerTestBuddy.DeleteAppPool("UnitTestAppPool");

            new IISManager().CreateAppPool("UnitTestAppPool", "pool-user", "pool-pass", RuntimeVersion.Version40);

            IISManagerTestBuddy.AssertAppPool("UnitTestAppPool", "pool-user", "pool-pass");
        }

        [TestMethod]
        [TestCategory("Integration")]
        public void Create_Website_Test()
        {
            IISManagerTestBuddy.DeleteWebsite("UnitTestWebsite");
            IISManagerTestBuddy.DeleteAppPool("UnitTestAppPool");

            new IISManager().CreateWebsite("UnitTestWebsite", @"c:\temp", 80, "UnitTestAppPool");

            IISManagerTestBuddy.AssertWebsite("UnitTestWebsite", @"c:\temp", 80, "UnitTestAppPool");
        }
    }
}