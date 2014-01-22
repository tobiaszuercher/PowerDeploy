using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PowerDeploy.PackageManagerExtension
{
    public class PowerShellBuddy
    {
        public static void ConfigureEnvironment(string environment, List<string> projects)
        {
            WriteHost("with runspace");
        }

        private static void WriteHost(string message)
        {
            var runspace = Runspace.DefaultRunspace;
            var pipeline = runspace.CreatePipeline("Write-Host '" + message + "'", false);
            pipeline.Invoke();
        }
    }
}