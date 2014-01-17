using System;
using System.Collections.Generic;
using System.Linq;

using PowerDeploy.Dashboard.DataAccess;
using PowerDeploy.Dashboard.Web.NuGetServer;

using Package = PowerDeploy.Dashboard.DataAccess.Entities.Package;

namespace PowerDeploy.Dashboard.Web.Providers
{
    public class PackageProvider
    {
        public int Synchronize()
        {
            var packageContext = new PackageContext(new Uri("http://localhost/nuggy/nuget"));
            int count = 0;

            var packages = from p in packageContext.Packages select p;

            using (var ctx = new Context())
            {
                foreach (var package in packages)
                {
                    if (!ctx.Packages.Any(p => p.NugetId == package.Id && p.Version == package.Version))
                    {
                        ++count;
                        ctx.Packages.Add(
                            new Package()
                            {
                                NugetId = package.Id,
                                Title = package.Title,
                                Version = package.Version,
                                Description = package.Description,
                                Published = package.Published,
                                Tags = package.Tags,
                                Authors = package.Authors,
                                IconUrl = package.IconUrl,
                                PackageSize = package.PackageSize,
                                ReleaseNotes = package.ReleaseNotes
                            });
                    }

                    ctx.SaveChanges();
                }
            }

            return count;
        }
    }
}