using System.IO;

namespace PowerDeploy.Core
{
    public interface IEnvironmentProvider
    {
        DirectoryInfo EnvironmentDirectory { get; }

        void Initialize(string startFolder);

        Environment GetEnvironment(string environmentName);
        Environment GetEnvironmentFromFile(string path);
    }
}