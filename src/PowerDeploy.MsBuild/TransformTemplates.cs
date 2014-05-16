using System.IO;
using Microsoft.Build.Utilities;

using PowerDeploy.Core;
using PowerDeploy.Core.Logging;
using PowerDeploy.Core.Template;

namespace PowerDeploy.MsBuild
{
    public class TransformTemplates : Task
    {
        public string Environment { get; set; }
        public string Directory { get; set; }

        public override bool Execute()
        {
            LogManager.LogFactory = new BuildLogFactory(Log);
            
            try
            {
                var envProvider = new EnvironmentProvider(Directory);

                var templateEngine = new TemplateEngine();
                templateEngine.TransformDirectory(Directory, envProvider.GetEnvironment(Environment), false);
            }
            catch (DirectoryNotFoundException)
            {
                Log.LogError(".powerdeploy folder not found for project " + Directory + "! :(");
                return false;
            }
            catch (FileNotFoundException exception)
            {
                Log.LogError(exception.Message);
                return false;
            }

            return true;
        }
    }
}
