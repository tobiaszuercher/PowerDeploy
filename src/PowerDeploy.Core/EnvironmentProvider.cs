using System;
using System.Collections;
using System.IO;
using System.Linq;

using PowerDeploy.Core.Extensions;
using PowerDeploy.Core.Logging;

namespace PowerDeploy.Core
{
    public class EnvironmentProvider : IEnvironmentProvider
    {
        private IEnvironmentSerializer Serializer { get; set; }

        public DirectoryInfo EnvironmentDirectory { get; private set; }

        public EnvironmentProvider()
        {
            Serializer = new XmlEnvironmentSerializer();
        }

        public EnvironmentProvider(string startFolder)
            : this()
        {
            Initialize(startFolder);
        }

        public void Initialize(string startFolder)
        {
            EnvironmentDirectory = FindEnvironmentFolder(startFolder);
        }

        public Environment GetEnvironmentFromFile(string environmentFile)
        {
            return Serializer.Deserialize(environmentFile);
        }

        public Environment GetEnvironment(string environmentName)
        {
            LogManager.GetLogger(GetType()).Debug("EnvironmentProvider.GetEnvironment");
            if (EnvironmentDirectory == null)
            {
                throw new InvalidOperationException("Please initialize EnvironmentProvider first.");    
            }

            if (EnvironmentDirectory.Exists)
            {
                var path = Path.Combine(EnvironmentDirectory.FullName, "{0}.xml".Fmt(environmentName));

                return GetEnvironmentFromFile(Path.Combine(EnvironmentDirectory.FullName, "{0}.xml".Fmt(environmentName)));
            }
            
            throw new DirectoryNotFoundException(".powerdeploy folder not found");
        }

        private DirectoryInfo FindEnvironmentFolder(string startFolder)
        {
            LogManager.GetLogger(GetType()).Debug("gugus " + startFolder);
            var dirInfo = new DirectoryInfo(startFolder);
            var root = Directory.GetDirectoryRoot(startFolder);

            while (dirInfo.FullName != root)
            {
                if (dirInfo.GetDirectories(".powerdeploy").Any())
                {
                    break;
                }

                if (dirInfo.Name == ".powerdeploy")
                {
                    dirInfo = dirInfo.Parent;
                    break;
                }

                dirInfo = Directory.GetParent(dirInfo.FullName);
            }

            var envFolder = new DirectoryInfo(Path.Combine(dirInfo.FullName, ".powerdeploy"));

            if (!envFolder.Exists)
            {
                throw new DirectoryNotFoundException(".powerdeploy folder not found with start folder " + startFolder);
            }

            return envFolder;
        }
    }
}