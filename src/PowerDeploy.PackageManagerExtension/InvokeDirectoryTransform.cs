using System.IO;
using System.Management.Automation;
using PowerDeploy.Core;
using PowerDeploy.Core.Logging;
using PowerDeploy.Core.Template;

namespace PowerDeploy.PackageManagerExtension
{
    [Cmdlet(VerbsLifecycle.Invoke, "DirectoryTransform")]
    public class InvokeDirectoryTransform : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Environment { get; set; }

        [Parameter(Mandatory = true)]
        public string Directory { get; set; }

        public static readonly ILog Log = LogManager.GetLogger(typeof(InvokeDirectoryTransform));
    
        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new PowerShellCommandLineLogFactory();

            try
            {
                var envProvider = new EnvironmentProvider(Directory);
                
                var templateEngine = new TemplateEngine();
                templateEngine.TransformDirectory(Directory, envProvider.GetEnvironment(Environment), false);
            }
            catch (DirectoryNotFoundException)
            {
                Log.Warn(".powerdeploy folder not found for project " + Directory + "! i'll skip it!");
            }
            catch (FileNotFoundException exception)
            {
                Log.Error(exception.Message);
            }
        }
    }
}