using System.IO;
using System.Net;

using PowerDeploy.Server.ServiceModel;

using ServiceStack;

namespace PowerDeploy.Server.Provider
{
    public class NugetPackageDownloader
    {
        public ServerSettings ServerSettings { get; set; }

        public string Downloaad(string packageId, string version, string targetPath)
        {
            using (var webclient = new WebClient())
            {
                var packagePath = Path.Combine(targetPath, "{0}_v{1}.nupkg".Fmt(packageId, version));

                webclient.DownloadFile(
                    "{0}/api/v2/package/{1}/{2}".Fmt(ServerSettings.NuGetServerUri, packageId, version),
                    packagePath);

                return packagePath;
            }
        }
    }
}