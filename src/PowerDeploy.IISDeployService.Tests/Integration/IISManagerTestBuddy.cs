using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Web.Administration;

namespace PowerDeploy.IISDeployService.Tests.Integration
{
    public class IISManagerTestBuddy
    {
        public static void DeleteAppPool(string name)
        {
            using (var manager = new ServerManager())
            {
                var pool = manager.ApplicationPools[name];

                if (pool != null)
                {
                    pool.Delete();
                    manager.CommitChanges();
                }
            }
        }

        public static void DeleteWebsite(string name)
        {
            using (var manager = new ServerManager())
            {
                var site = manager.Sites[name];

                if (site != null)
                {
                    site.Delete();
                    manager.CommitChanges();
                }
            }
        }

        public static void AssertAppPool(string name, string user, string pass)
        {
            using (var manager = new ServerManager())
            {
                Assert.IsNotNull(manager.ApplicationPools[name]);
                Assert.AreEqual(name, manager.ApplicationPools[name].Name);
                Assert.AreEqual(user, manager.ApplicationPools[name].ProcessModel.UserName);
                Assert.AreEqual(pass, manager.ApplicationPools[name].ProcessModel.Password);
            }
        }

        public static void AssertWebsite(string name, string path, int port, string appPoolName)
        {
            using (var manager = new ServerManager())
            {
                var site = manager.Sites[name];
                
                Assert.IsNotNull(site, "Site is null");

                Assert.AreEqual(path, site.Applications["/"].VirtualDirectories["/"].PhysicalPath);
                Assert.AreEqual(appPoolName, site.ApplicationDefaults.ApplicationPoolName);
                Assert.AreEqual(port, site.Bindings[0].EndPoint.Port);
            }   
        }
    }
}