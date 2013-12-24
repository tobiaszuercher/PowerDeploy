using System;
using System.Globalization;
using System.IO;
using System.Linq;

using Ionic.Zip;

using NuGet;

using PowerDeploy.Core.Template;

namespace PowerDeploy.Core
{
    public class PackageManager
    {
        private readonly IFileSystem _fileSystem;
        private readonly TemplateEngine _templateEngine;

        public IEnviornmentProvider EnviornmentProvider { get; set; }

        public PackageManager(IEnviornmentProvider environmentProvider)
            : this(new PhysicalFileSystem(), environmentProvider)
        {
        }

        public PackageManager(IFileSystem fileSystem, IEnviornmentProvider environmentProvider)
        {
            _fileSystem = fileSystem;
            _templateEngine = new TemplateEngine(fileSystem, environmentProvider);
            EnviornmentProvider = environmentProvider;
        }

        public void ConfigurePackage(string packagePath, string environment, string outputPath)
        {
            Console.WriteLine("Configuring package {0} for {1}", new FileInfo(packagePath).Name, environment.ToUpper());
            var workingDir = _fileSystem.CreateTempWorkingDir();

            // read nupkg metadata
            var nupkg = new ZipPackage(packagePath);

            using (var zip = new ZipFile(packagePath))
            {
                zip.ExtractAll(workingDir);
            }

            _templateEngine.TransformDirectory(workingDir, environment);
            var packageName = nupkg.Id + "_" + nupkg.Version + "_" + environment.ToUpper(CultureInfo.InvariantCulture) + ".nupkg";
            _fileSystem.DeleteFile(packageName);

            using (var zip = new ZipFile(Path.Combine(outputPath, packageName)))
            {
                zip.AddDirectory(workingDir);
                zip.Save();
            }

            _fileSystem.DeleteDirectory(workingDir);
        }

        public void DeployPackage(string package)
        {
            var nupkg = new ZipPackage(package);
            
        }
    }
}