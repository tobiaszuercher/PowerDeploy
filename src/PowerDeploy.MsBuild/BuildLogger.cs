using System;

using Microsoft.Build.Utilities;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.MsBuild
{
    public class BuildLogger : ILog
    {
        public bool IsDebugEnabled { get; private set; }

        public TaskLoggingHelper _helper;

        public BuildLogger(TaskLoggingHelper cmdlet)
        {
            _helper = cmdlet;
        }

        public void Debug(object message)
        {
            _helper.LogCommandLine(message.ToString());
        }

        public void Debug(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void DebugFormat(string format, params object[] args)
        {
            _helper.LogCommandLine(string.Format(format, args));
        }

        public void Print(object message)
        {
            _helper.LogCommandLine(message.ToString());
        }

        public void PrintFormat(string format, params object[] args)
        {
            _helper.LogCommandLine(string.Format(format, args));
        }

        public void Error(object message)
        {
            _helper.LogCommandLine(message.ToString());
        }

        public void Error(object message, Exception exception)
        {
            _helper.LogCommandLine(message.ToString());
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _helper.LogCommandLine(string.Format(format, args));
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
            _helper.LogCommandLine(message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void InfoFormat(string format, params object[] args)
        {
            _helper.LogCommandLine(string.Format(format, args));
        }

        public void Warn(object message)
        {
            _helper.LogCommandLine(message.ToString());
        }

        public void Warn(object message, Exception exception)
        {
            throw new NotImplementedException();
        }

        public void WarnFormat(string format, params object[] args)
        {
            _helper.LogCommandLine(string.Format(format, args));
        }
    }
}