using System.Collections.Generic;
using System.IO;

namespace PowerDeploy.Core
{
    public interface IEnvironmentProvider
    {
        DirectoryInfo EnvironmentDirectory { get; }

        void Initialize(string startFolder);

        IEnumerable<string> GetEnvironments(bool excludeShared = true);
        Environment GetEnvironment(string environmentName);
        Environment GetEnvironmentFromFile(string environmentFile);
    }
}