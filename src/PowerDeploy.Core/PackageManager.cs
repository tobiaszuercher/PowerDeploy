using System.Globalization;
using System.IO;

using Ionic.Zip;

using NuGet;

using PowerDeploy.Core.Extensions;
using PowerDeploy.Core.Logging;
using PowerDeploy.Core.Template;

namespace PowerDeploy.Core
{
    public class PackageManager
    {
        private readonly IFileSystem _fileSystem;

        private readonly TemplateEngine _templateEngine;

        private readonly ILog _logger;

        public IEnvironmentParser EnvironmentParser { get; set; }

        private PluginLoader _pluginsLoader;

        private string _environmentDir;

        public PackageManager(string environmentDir)
            : this(new PhysicalFileSystem(), environmentDir, new XmlEnvironmentParser(environmentDir))
        {
        }

        public PackageManager(IEnvironmentParser environmentParser)
            : this(new PhysicalFileSystem(), string.Empty, environmentParser)
        {
        }

        public PackageManager(IFileSystem fileSystem, string environmentDir, IEnvironmentParser environmentParser)
        {
            _fileSystem = fileSystem;
            _templateEngine = new TemplateEngine(fileSystem);
            EnvironmentParser = environmentParser;
            _logger = LogManager.GetLogger(GetType());
            _pluginsLoader = new PluginLoader();
            _environmentDir = environmentDir;
        }

        public void ConfigurePackageByEnvironment(string packagePath, string environment, string outputPath)
        {
            var targetEnvironment = EnvironmentParser.GetEnvironment(environment);

            DoConfigure(packagePath, targetEnvironment, outputPath);
        }

        public void ConfigurePackage(string packageFile, string environmentFile, string outputPath)
        {
            var targetEnvironment = EnvironmentParser.GetEnvironmentFromFile(environmentFile);

            DoConfigure(packageFile, targetEnvironment, outputPath);
        }

        private void DoConfigure(string packagePath, Environment env, string outputPath)
        {
            _logger.InfoFormat("Configuring package {0} for {1}", new FileInfo(packagePath).Name, env.Name.ToUpper());
            var workingDir = _fileSystem.CreateTempWorkingDir();

            _logger.DebugFormat("Create temp work dir {0}", workingDir);

            // read nupkg metadata
            var nupkg = new ZipPackage(packagePath);
            _logger.DebugFormat("Unzipping {0} to {1}", nupkg.GetFullName(), workingDir);

            using (var zip = new ZipFile(packagePath))
            {
                zip.ExtractAll(workingDir);
            }

            _templateEngine.TransformDirectory(workingDir, env);
            var packageName = nupkg.Id + "_v" + nupkg.Version + "_" + env.Name.ToUpper(CultureInfo.InvariantCulture) + ".nupkg";
            var packageOutputPath = Path.Combine(outputPath, packageName);

            _fileSystem.DeleteFile(packageOutputPath);

            using (var zip = new ZipFile(packageOutputPath))
            {
                zip.AddDirectory(workingDir);
                zip.Save();
            }

            _fileSystem.DeleteDirectory(workingDir);
        }

        public void DeployPackage(string package)
        {
            var nupkg = new ZipPackage(package);
            var packageType = nupkg.PeekPackageType();

            _logger.Info("Deploy {1} package {0}".Fmt(packageType, package));

            foreach (var deployer in _pluginsLoader.Deployers)
            {
                if (deployer.Metadata.PackageType == packageType)
                {
                    deployer.Value.Deploy(nupkg, _logger);

                    break;
                }
            }
        }
    }
}