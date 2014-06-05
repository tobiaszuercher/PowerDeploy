using System;
using System.Management.Automation.Runspaces;
using PowerDeploy.Core.Logging;

namespace PowerDeploy.PackageManagerExtension
{
    public class PowerShellConsoleLogger : ILog
    {
        public bool IsDebugEnabled { get; private set; }

        public void Debug(object message)
        {
            ExecutePsCommand("Write-Verbose", message.ToString());
        }

        public void Debug(object message, Exception exception)
        {
            ExecutePsCommand("Write-Verbose", message + " " + exception.Message);
        }

        public void DebugFormat(string format, params object[] args)
        {
            ExecutePsCommand("Write-Verbose", string.Format(format, args));
        }

        public void Print(object message)
        {
            ExecutePsCommand("Write-Host", message.ToString());
        }

        public void PrintFormat(string format, params object[] args)
        {
            ExecutePsCommand("Write-Host", string.Format(format, args));
        }

        public void Error(object message)
        {
            ExecutePsCommand("Write-Error", message.ToString());
        }

        public void Error(object message, Exception exception)
        {
            ExecutePsCommand("Write-Error", message + Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            ExecutePsCommand("Write-Error", string.Format(format, args));
        }

        public void Fatal(object message)
        {
            ExecutePsCommand("Write-Error", "FATAL: " + message);
        }

        public void Fatal(object message, Exception exception)
        {
            ExecutePsCommand("Write-Error", "FATAL: " + message + " " + exception.Message);
        }

        public void FatalFormat(string format, params object[] args)
        {
            ExecutePsCommand("Write-Error", string.Format("FATAL: " + format, args));
        }

        public void Info(object message)
        {
            ExecutePsCommand("Write-Host", message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            ExecutePsCommand("Write-Host", message + " " + exception.Message);
        }

        public void InfoFormat(string format, params object[] args)
        {
            ExecutePsCommand("Write-Host", string.Format(format, args));
        }

        public void Warn(object message)
        {
            ExecutePsCommand("Write-Warning", message.ToString());
        }

        public void Warn(object message, Exception exception)
        {
            ExecutePsCommand("Write-Warning", message + " " + exception.Message);
        }

        public void WarnFormat(string format, params object[] args)
        {
            ExecutePsCommand("Write-Warning", string.Format(format, args));
        }

        private void ExecutePsCommand(string cmd, string message)
        {
            var runspace = Runspace.DefaultRunspace;
            
            var pipeline = runspace.CreateNestedPipeline(string.Format(@"{0} ""{1}""", cmd, message), false);
            
            pipeline.Invoke();
        }
    }
}