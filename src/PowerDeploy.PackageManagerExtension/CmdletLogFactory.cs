using System;
using System.Management.Automation;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.PackageManagerExtension
{
    /// <summary>
    /// Uses a cmdlet to log to
    /// 
    /// Made public so its testable
    /// </summary>
    /// <remarks>https://github.com/ServiceStackV3/ServiceStackV3 BSD Licence.</remarks>
    public class CmdletLogFactory : ILogFactory
    {
        private readonly Cmdlet _cmdlet;
        
        public CmdletLogFactory(Cmdlet cmdlet)
        {
            _cmdlet = cmdlet;
        }

        public ILog GetLogger(Type type)
        {
            return new CmdletLogger(_cmdlet);
        }

        public ILog GetLogger(string typeName)
        {
            return new CmdletLogger(_cmdlet);
        }
    }
}