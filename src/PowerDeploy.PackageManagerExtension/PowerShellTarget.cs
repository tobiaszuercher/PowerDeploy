using System.Management.Automation.Runspaces;
using NLog;
using NLog.Targets;

namespace PowerDeploy.PackageManagerExtension
{
    [Target("MyFirst")]
    public sealed class MyFirstTarget : TargetWithLayout
    {
        public MyFirstTarget()
        {
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);

            if (logEvent.Level.Equals(LogLevel.Debug))
            {
                //ExecutePsCommand("Write-Verbose);
            }

        }

        private void ExecutePsCommand(string cmd, string message)
        {
            var runspace = Runspace.DefaultRunspace;
            var pipeline = runspace.CreateNestedPipeline(string.Format("{0} '{1}'", cmd, message), false);
            pipeline.Invoke();
        }
    } 
}