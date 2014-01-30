using System.IO;
using System.Management.Automation;

using PowerDeploy.Core.Logging;

using EnvironmentProvider = PowerDeploy.Core.EnvironmentProvider;

namespace PowerDeploy.PackageManagerExtension
{
    [Cmdlet(VerbsCommon.Get, "EnvironmentDir")]
    public class GetEnvironmentDir : PSCmdlet
    {
        [Parameter(Mandatory = false, Position = 1)]
        public string Directory { get; set; }

        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new CmdletLogFactory(this);

            if (string.IsNullOrEmpty(Directory))
            {
                Directory = SessionState.Path.CurrentLocation.Path;
            }

            EnvironmentProvider envProvider;
            
            try
            {
                envProvider = new EnvironmentProvider(Directory);
            }
            catch (DirectoryNotFoundException)
            {
                LogManager.GetLogger(GetType()).Warn(".powerdeploy folder not found!");
                
                return;
            }

            WriteObject(envProvider.EnvironmentDirectory);
        }
    }
}