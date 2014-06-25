using System.IO;
using System.Management.Automation;

using PowerDeploy.Core;
using PowerDeploy.Core.Logging;
using PowerDeploy.Core.Template;

using Environment = PowerDeploy.Core.Environment;

namespace PowerDeploy.PackageManagerExtension
{
    [Cmdlet(VerbsLifecycle.Invoke, "DirectoryTransform")]
    public class InvokeDirectoryTransform : PSCmdlet
    {
        [Parameter]
        public string Environment { get; set; }

        [Parameter]
        public string EnvironmentFile { get; set; }

        [Parameter(Mandatory = true)]
        public string Directory { get; set; }

        public static ILog Log { get; private set; }
    
        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new PowerShellCommandLineLogFactory();
            Log = LogManager.GetLogger(typeof (InvokeDirectoryTransform));

            try
            {
                // TODO: make that nice, i'm in hurry right know :)
                EnvironmentProvider envProvider = null;

                envProvider = string.IsNullOrEmpty(Environment) ? new EnvironmentProvider() : new EnvironmentProvider(Directory);

                var templateEngine = new TemplateEngine();

                Environment env = !string.IsNullOrEmpty(Environment) ? envProvider.GetEnvironment(Environment) : envProvider.GetEnvironmentFromFile(EnvironmentFile);
                
                templateEngine.TransformDirectory(Directory, env, false);
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