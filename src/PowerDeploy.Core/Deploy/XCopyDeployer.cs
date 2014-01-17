using System.IO;

using PowerDeploy.Core.Extensions;
using PowerDeploy.Core.Logging;

using ZipPackage = NuGet.ZipPackage;

namespace PowerDeploy.Core.Deploy
{
    [DeployerMetadataAttribute(PackageType = "xcopy")]
    public class XCopyDeployer : IDeployer
    {
        private readonly IFileSystem _fileSystem;

        public XCopyDeployer()
        {
            _fileSystem = new PhysicalFileSystem();
        }

        public bool Deploy(ZipPackage package, ILog logger)
        {
            var options = package.GetPackageOptions<XCopyOptions>();

            if (!_fileSystem.DirectoryExists(options.Destination))
            {
                logger.Error("{0} doesn't exist. Please create the folder before deploying!");

                return false;
            }

            logger.Info("Deploying xcopy package to {0}".Fmt(options.Destination));

            foreach (var file in package.GetFiles())
            {
                _fileSystem.EnsureDirectoryExists(options.Destination);

                using (var stream = file.GetStream())
                {
                    var destination = Path.Combine(options.Destination, file.Path);

                    new FileInfo(destination).Directory.Create(); // ensure directory is created
                    FileStream targetFile = File.OpenWrite(destination);
                    stream.CopyTo(targetFile);
                    targetFile.Close();
                }
            }

            return true;
        }
    }
}