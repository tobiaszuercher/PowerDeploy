using System;
using System.Management.Automation;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.PackageManagerExtension
{
    public class CmdletLogger : ILog
    {
        public bool IsDebugEnabled { get; private set; }

        private readonly Cmdlet _cmdLet;

        public CmdletLogger(Cmdlet cmdlet)
        {
            _cmdLet = cmdlet;
        }

        public void Debug(object message)
        {
            _cmdLet.WriteDebug(message.ToString());
        }

        public void Debug(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void DebugFormat(string format, params object[] args)
        {
            _cmdLet.WriteDebug(string.Format(format, args));
        }

        public void Print(object message)
        {
            _cmdLet.WriteObject(message);
        }

        public void PrintFormat(string format, params object[] args)
        {
            _cmdLet.WriteObject(string.Format(format, args));
        }

        public void Error(object message)
        {
            _cmdLet.WriteWarning("ERROR: " + message);
        }

        public void Error(object message, Exception exception)
        {
            _cmdLet.WriteWarning("EXCEPTION: " + message + Environment.NewLine + exception.Message + Environment.NewLine + exception.StackTrace);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _cmdLet.WriteWarning(string.Format(format, args));
        }

        public void Fatal(object message)
        {
            throw new NotImplementedException();
        }

        public void Fatal(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void FatalFormat(string format, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Info(object message)
        {
            _cmdLet.WriteObject(message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void InfoFormat(string format, params object[] args)
        {
            _cmdLet.WriteVerbose(string.Format(format, args));
        }

        public void Warn(object message)
        {
            _cmdLet.WriteWarning(message.ToString());
        }

        public void Warn(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, params object[] args)
        {
            _cmdLet.WriteWarning(string.Format(format, args));
        }
    }
}