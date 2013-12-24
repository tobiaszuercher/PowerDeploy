using System;
using System.IO;

using Microsoft.Web.Administration;

using PowerDeploy.IISDeployService.Contract;

using ServiceStack.Text;

namespace PowerDeploy.IISDeployService
{
    /// <summary>
    /// Helps create app pools and websites.
    /// </summary>
    public class IISManager
    {
        public void CreateAppPool(string name, string user, string pass, RuntimeVersion version, bool forceOverwrite = false)
        {
            using (var manager = new ServerManager()) // create app pool if not exists
            {
                if (manager.ApplicationPools[name] == null || forceOverwrite)
                {
                    var pool = manager.ApplicationPools.Add(name);
                    pool.ProcessModel.IdentityType = ProcessModelIdentityType.SpecificUser;
                    pool.ProcessModel.UserName = user;
                    pool.ProcessModel.Password = pass;
                    pool.ManagedRuntimeVersion = "v".Fmt(version.ToVersionNumber());
                    manager.CommitChanges();
                }
            }
        }

        public void CreateWebsite(string name, string physicalPath, int port, string appPoolName, bool forceOverwrite = false)
        {
            using (var manager = new ServerManager())
            {
                if (manager.Sites[name] == null || forceOverwrite)
                {
                    if (!Directory.Exists(physicalPath))
                    {
                        Directory.CreateDirectory(physicalPath);
                    }

                    var site = manager.Sites.Add(name, physicalPath, port);
                    site.ApplicationDefaults.ApplicationPoolName = appPoolName;
                    manager.CommitChanges();
                }
            }
        }

        public void MapWebsitePhysicalPath(string websiteName, string physicalPath)
        {
            using (var manager = new ServerManager())
            {
                if (manager.Sites[websiteName] == null)
                {
                    throw new ArgumentException("The given website doesn't exist.", "websiteName");    
                }

                manager.Sites[websiteName].Applications["/"].VirtualDirectories["/"].PhysicalPath = physicalPath;
                manager.CommitChanges();
            }
        }

        public void CreateApplication(string siteName, string path, string physicalPath)
        {
            using (var manager = new ServerManager())
            {
                var site = manager.Sites[siteName];

                var application = site.Applications[path];

                if (application == null)
                {
                    site.Applications.Add(path, physicalPath);
                }
                else
                {
                    application.VirtualDirectories["/"].PhysicalPath = physicalPath;
                }

                manager.CommitChanges();
            }
        }

        public string GetWebsitePath(string sitename)
        {
            using (var manager = new ServerManager())
            {
                var site = manager.Sites[sitename];

                if (site == null)
                {
                    throw new ArgumentException("Site {0} doesn't exist.".Fmt(sitename), "sitename");
                }

                return site.Applications["/"].VirtualDirectories["/"].PhysicalPath;
            }
        }

        public string GetApplicationPath(string siteName, string appRoot, string appName)
        {
            using (var manager = new ServerManager())
            {
                var site = manager.Sites[siteName];

                if (site == null)
                {
                    throw new ArgumentException("Site {0} doesn't exist.".Fmt(siteName), "siteName");
                }

                var appPath = "/{0}/{1}".Fmt(appRoot.Trim('/'), appName);
                var app = site.Applications[appPath];

                if (app == null)
                {
                    throw new ArgumentException("Application {0} doesn't exist.".Fmt(appPath));
                }

                return app.VirtualDirectories["/"].PhysicalPath;
            }
        }

        public void RollbackApplication(string siteName, string appRoot, string appName, string rollbackTarget)
        {
            using (var manager = new ServerManager())
            {
                var site = manager.Sites[siteName];

                if (site == null)
                {
                    throw new ArgumentException("Site {0} doesn't exist.".Fmt(siteName), "siteName");
                }

                var appPath = "/{0}/{1}".Fmt(appRoot.Trim('/'), appName);
                var app = site.Applications[appPath];

                if (app == null)
                {
                    throw new ArgumentException("Application {0} doesn't exist.".Fmt(appPath));
                }
                
                var parentPath = Directory.GetParent(app.VirtualDirectories["/"].PhysicalPath);

                app.VirtualDirectories["/"].PhysicalPath = Path.Combine(parentPath.FullName, rollbackTarget);

                manager.CommitChanges();
            }
        }
    }
}