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

        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new CmdletLogFactory(this);
            var logger = LogManager.GetLogger(GetType());
            logger.DebugFormat("Invoke-DirectoryTransform for environment {0} in {1}", Environment, Directory);

            try
            {
                var envProvider = new EnvironmentProvider(Directory);
                
                var templateEngine = new TemplateEngine();
                templateEngine.TransformDirectory(Directory, envProvider.GetEnvironment(Environment), false);
            }
            catch (DirectoryNotFoundException)
            {
                logger.Warn(".powerdeploy folder not found for project " + Directory + "! i'll skip it!");
            }
            catch (FileNotFoundException exception)
            {
                logger.Error(exception.Message);
            }
        }
    }
}