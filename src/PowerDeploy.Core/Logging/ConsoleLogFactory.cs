using System;

namespace PowerDeploy.Core.Logging
{
    /// <summary>
    /// Creates a Console Logger, that logs all messages to: System.Console
    /// 
    /// Made public so its testable
    /// </summary>
    /// <remarks>https://github.com/ServiceStackV3/ServiceStackV3 BSD Licence.</remarks>
	public class ConsoleLogFactory : ILogFactory
    {
        public ILog GetLogger(Type type)
        {
            return new ConsoleLogger(type);
        }

        public ILog GetLogger(string typeName)
        {
			return new ConsoleLogger(typeName);
        }
    }
}