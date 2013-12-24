using System;

namespace PowerDeploy.Core.Logging
{
    /// <summary>
    /// Creates a Debug Logger, that logs all messages to: System.Diagnostics.Debug
    /// 
    /// Made public so its testable
    /// </summary>
    /// <remarks>https://github.com/ServiceStackV3/ServiceStackV3 BSD Licence.</remarks>
    public class NullLogFactory : ILogFactory
    {
        public ILog GetLogger(Type type)
        {
            return new NullLogger(type);
        }

        public ILog GetLogger(string typeName)
        {
            return new NullLogger(typeName);
        }
    }
}