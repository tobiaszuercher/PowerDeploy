using System;
using System.Linq;

using PowerDeploy.Server.NuGetServer;

using Raven.Client;

using ServiceStack;

namespace PowerDeploy.Server.Provider
{
    public class NugetServerBridge
    {
        public IDocumentStore DocumentStore { get; set; }

        public SynchronizationSummary Synchronize()
        {
            var packageContext = new PackageContext(new Uri("http://localhost/nuggy/nuget"));
            int addedPackages = 0;

            var packages = from p in packageContext.Packages select p;

            using (var session = DocumentStore.OpenSession())
            {               
                foreach (var nugetPackage in packages)
                {
                    var package = session.Load<PowerDeploy.Server.Model.Package>("packages/" + nugetPackage.Id + "/" + nugetPackage.Version);

                    if (package == null)
                    {
                        ++addedPackages;

                        var packageInfo = new PowerDeploy.Server.Model.Package().PopulateWith(nugetPackage);
                        packageInfo.NugetId = nugetPackage.Id;
                        packageInfo.Id = "packages/" + nugetPackage.Id + "/" + nugetPackage.Version;

                        session.Store(packageInfo);
                    }
                }

                session.SaveChanges();
            }

            return new SynchronizationSummary() { AddedPackages = addedPackages, PackagesInNuget = packages.Count() };
        }
    }
}