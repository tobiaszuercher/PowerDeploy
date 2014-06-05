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

        public static ILog Log { get; private set; }
    
        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new PowerShellCommandLineLogFactory();
            Log = LogManager.GetLogger(typeof (InvokeDirectoryTransform));

            try
            {
                var envProvider = new EnvironmentProvider(Directory);
                
                var templateEngine = new TemplateEngine();
                templateEngine.TransformDirectory(Directory, envProvider.GetEnvironment(Environment), false);
            }
            catch (DirectoryNotFoundException)
            {
                Log.Warn(".powerdeploy folder not found for " + Directory + "!");
            }
            catch (FileNotFoundException exception)
            {
                Log.Error(exception.Message);
            }
        }
    }
}