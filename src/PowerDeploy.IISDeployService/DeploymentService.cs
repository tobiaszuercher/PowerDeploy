using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

using Ionic.Zip;

using PowerDeploy.IISDeployService.Contract;

using ServiceStack;
using ServiceStack.Configuration;

namespace PowerDeploy.IISDeployService
{
    /// <summary>
    /// There are two ways to deploy:
    /// 
    /// First way (deploy to the root)
    /// deploy the package to the root of the website
    /// example: $WebSiteRoot = C:\inetpub\Website
    ///
    /// it copies the package to $WebSiteRoot\%PackageName%_%Version% and map the Website to the folder.
    /// Example:
    ///
    /// C:\inetpub\Website\Website_v1.0.0
    /// C:\inetpub\Website\Website_v1.1.0
    ///   
    /// The deployment process for "Root-Website"-Deployments just copies the package to the $WebsiteRoot 
    /// and maps the WebSite to this folder.
    /// </summary>
    [ApiKeyAuth]
    public class DeploymentService : Service
    {
        public IISManager IISManager { get; set; }

        public IAppSettings Settings { get; set; }

        public object Post(TriggerDeployment request)
        {
            

            if (Request.Files.Length != 1)
            {
                throw new HttpError(HttpStatusCode.BadRequest, "Please add one (and just one) file to deploy.");
            }

            IISManager.CreateAppPool(request.AppPoolName, request.AppPoolUser, request.AppPoolPassword, request.RuntimeVersion, request.Overwrite);
            IISManager.CreateWebsite(request.WebsiteName, request.WebsitePhysicalPath, request.WebsitePort, request.AppPoolName, request.Overwrite);

            var uploadedPackageZip = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "package.zip");
            Request.Files[0].SaveTo(uploadedPackageZip);

            if (string.IsNullOrEmpty(request.AppRoot) || request.AppRoot == "/")
            {
                var targetPath = Path.Combine(request.WebsitePhysicalPath, "{0}_v{1}".Fmt(request.PackageId, request.PackageVersion));

                using (var zip = ZipFile.Read(uploadedPackageZip))
                {
                    zip.ForEach(z => z.Extract(targetPath, ExtractExistingFileAction.OverwriteSilently));
                }

                // the package is unpacked to the new destination, remap the website to the new version
                IISManager.MapWebsitePhysicalPath(request.WebsiteName, targetPath);
            }
            else
            {
                //// lets deploy in a more complex scenario
                ////
                //// example:
                ////  WebsiteName:         Default Web Site
                ////  WebsitePhysicalPath: c:\inetpub\root
                ////  AppRoot:             "sub1/sub2"
                ////  AppName:             "app"
                ////  AppPhysicalPath:     "c:\iis_apps\app"
                ////
                //// What happens:
                //// c:\inetpub\root\sub1\sub2 will be created physically. In this folder, it creates an WebApplication Named "app" pointing to "c:\iis_apps\app"
                //// Each deployment will add a subfolder in "c:\iis_apps\app": 
                ////   c:\iis_apps\app\app_v1.0.1.0
                ////   c:\iis_apps\app\app_v1.0.1.1
                ////   c:\iis_apps\app\app_v1.0.1.2

                ////this lets you access your application with for example http://your-host/sub1/sub2/app
                if (!Directory.Exists(request.AppPhysicalPath))
                {
                    Directory.CreateDirectory(request.AppPhysicalPath);
                }

                var appTargetPath = Path.Combine(request.AppPhysicalPath, "{0}_v{1}".Fmt(request.PackageId, request.PackageVersion));

                using (var zip = ZipFile.Read(uploadedPackageZip))
                {
                    zip.ForEach(z => z.Extract(appTargetPath, ExtractExistingFileAction.OverwriteSilently));
                }

                ////ApplyBackupRetentionPolicy(request.AppPhysicalPath, request.AppName);

                var appFolderInWebsite = Path.Combine(request.WebsitePhysicalPath, request.AppRoot.Trim('/'));
                var appPath = "{0}/{1}".Fmt(request.AppRoot.TrimEnd('/'), request.AppName);

                if (!Directory.Exists(appFolderInWebsite))
                {
                    Directory.CreateDirectory(appFolderInWebsite);
                }

                IISManager.CreateApplication(request.WebsiteName, appPath, appTargetPath);
            }

            return new TriggerDeploymentResponse { Status = "OK" };
        }

        private void ApplyBackupRetentionPolicy(string path, string folderPrefix)
        {
            new DirectoryInfo(path).GetDirectories("{0}_v*".Fmt(folderPrefix))
                .OrderByDescending(d => d.CreationTimeUtc)
                .Skip(Settings.Get("backup.history", 5))
                .ForEach(d => d.Delete(true));
        }

        public List<DeploymentInfo> Get(QueryDeployments request)
        {
            string path;

            if (!string.IsNullOrEmpty(request.AppName) && !string.IsNullOrEmpty(request.AppRoot))
            {
                path = Path.Combine(IISManager.GetApplicationPath(request.WebsiteName, request.AppRoot, request.AppName), "..");
            }
            else
            {
                path = Path.Combine(IISManager.GetWebsitePath(request.WebsiteName), "..");
            }

            var list = new List<DeploymentInfo>();

            new DirectoryInfo(path).GetDirectories().ForEach(d => list.Add(new DeploymentInfo { Name = d.Name }));

            return list;
        }

        public void Post(RollbackDeployment request)
        {
            if (string.IsNullOrEmpty(request.AppName) && string.IsNullOrEmpty(request.AppRoot))
            {
                var websitePath = IISManager.GetWebsitePath(request.WebsiteName);

                IISManager.MapWebsitePhysicalPath(request.WebsiteName, Path.Combine(Directory.GetParent(websitePath).FullName, request.RollbackTarget));
            }
            else
            {
                IISManager.RollbackApplication(request.WebsiteName, request.AppRoot, request.AppName, request.RollbackTarget);
            }
        }
    }
}