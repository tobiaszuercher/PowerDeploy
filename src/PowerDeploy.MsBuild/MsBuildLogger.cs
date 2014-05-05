using System;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.MsBuild
{
    public class MsBuildLogger : ILog
    {
        public bool IsDebugEnabled { get; private set; }

        public TaskLoggingHelper _msbuildLogger;

        public MsBuildLogger(TaskLoggingHelper cmdlet)
        {
            _msbuildLogger = cmdlet;
        }

        public void Debug(object message)
        {
            _msbuildLogger.LogMessage(message.ToString());
        }

        public void Debug(object message, Exception exception)
        {
            _msbuildLogger.LogErrorFromException(exception, true);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _msbuildLogger.LogMessage(format, args);
        }

        public void Print(object message)
        {
            _msbuildLogger.LogCommandLine(MessageImportance.High, message.ToString());
        }

        public void PrintFormat(string format, params object[] args)
        {
            _msbuildLogger.LogCommandLine(MessageImportance.High, string.Format(format, args));
        }

        public void Error(object message)
        {
            _msbuildLogger.LogError(message.ToString());
        }

        public void Error(object message, Exception exception)
        {
            _msbuildLogger.LogError(message.ToString());
            _msbuildLogger.LogErrorFromException(exception, true);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _msbuildLogger.LogError(format, args);
        }

        public void Fatal(object message)
        {
            _msbuildLogger.LogError(message.ToString());
        }

        public void Fatal(object message, Exception exception)
        {
            _msbuildLogger.LogError(message.ToString());
            _msbuildLogger.LogErrorFromException(exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _msbuildLogger.LogError(format, args);
        }

        public void Info(object message)
        {
            _msbuildLogger.LogMessage(MessageImportance.High, message.ToString());
        }

        public void Info(object message, Exception exception)
        {
            _msbuildLogger.LogError(message.ToString());
            _msbuildLogger.LogErrorFromException(exception, true);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _msbuildLogger.LogCommandLine(MessageImportance.High, string.Format(format, args));
        }

        public void Warn(object message)
        {
            _msbuildLogger.LogCommandLine(message.ToString());
        }

        public void Warn(object message, Exception exception)
        {
            _msbuildLogger.LogWarning(message.ToString());
            _msbuildLogger.LogWarningFromException(exception, true);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _msbuildLogger.LogWarning(format, args);
        }
    }
}