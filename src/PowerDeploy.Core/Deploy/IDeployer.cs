using NuGet;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.Core.Deploy
{
    public interface IDeployer
    {
        bool Deploy(ZipPackage package, ILog logger);
    }
}