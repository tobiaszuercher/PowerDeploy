using System;

using Microsoft.Build.Utilities;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.MsBuild
{
    /// <summary>
    /// Uses a cmdlet to log to
    /// 
    /// Made public so its testable
    /// </summary>
    /// <remarks>https://github.com/ServiceStackV3/ServiceStackV3 BSD Licence.</remarks>
    public class BuildLogFactory : ILogFactory
    {
        private readonly TaskLoggingHelper _taskLogHelper;

        public BuildLogFactory(TaskLoggingHelper taskLogHelper)
        {
            _taskLogHelper = taskLogHelper;
        }

        public ILog GetLogger(Type type)
        {
            return new BuildLogger(_taskLogHelper);
        }

        public ILog GetLogger(string typeName)
        {
            return new BuildLogger(_taskLogHelper);
        }
    }
}
