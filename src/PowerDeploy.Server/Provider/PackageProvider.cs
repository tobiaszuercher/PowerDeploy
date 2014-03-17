using System;
using System.Linq;

using PowerDeploy.Server.NuGetServer;
using PowerDeploy.Server.ServiceModel;

using Raven.Client;

using ServiceStack;

namespace PowerDeploy.Server.Provider
{
    public class PackageProvider
    {
        public IDocumentStore DocumentStore { get; set; }

        public SynchronizeSummary Synchronize()
        {
            var packageContext = new PackageContext(new Uri("http://localhost/nuggy/nuget"));
            int addedPackages = 0;

            var packages = from p in packageContext.Packages select p;

            using (var session = DocumentStore.OpenSession())
            {               
                foreach (var package in packages)
                {
                    if (!session.Query<PackageInfo>().Any(pi => package.Id == pi.NugetId && package.Version == pi.Version))
                    {
                        ++addedPackages;

                        var packageInfo = new PackageInfo().PopulateWith(package);
                        packageInfo.NugetId = package.Id;

                        session.Store(packageInfo);
                    }
                }

                session.SaveChanges();
            }

            return new SynchronizeSummary() { AddedPackages = addedPackages, TotalPackages = packages.Count() };
        }
    }
}