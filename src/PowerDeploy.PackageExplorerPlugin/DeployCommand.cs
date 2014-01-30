using System.Windows.Forms;

using NuGet;

using NuGetPackageExplorer.Types;

using PowerDeploy.Core;

namespace PowerDeploy.PackageExplorerPlugin
{
    [PackageCommandMetadata("PowerDeploy")]
    internal class DeployCommand : IPackageCommand
    {
        public void Execute(IPackage package, string packagePath)
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                var provider = new EnvironmentProvider();

                var packageManager = new PackageManager(provider);
                packageManager.ConfigurePackage(packagePath, dialog.FileNames[0], @"c:\temp\packageexplorer");
            }
        }
    }
}