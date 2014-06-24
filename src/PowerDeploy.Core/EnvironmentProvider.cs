using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PowerDeploy.Core.Logging;

using ServiceStack;
using ServiceStack.Text;

namespace PowerDeploy.Core
{
    public class EnvironmentProvider : IEnvironmentProvider
    {
        private IEnvironmentSerializer Serializer { get; set; }

        private ILog Log = LogManager.GetLogger(typeof (EnvironmentProvider));

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
            var environment = Serializer.Deserialize(environmentFile);

            // include other environments if defined
            if (!string.IsNullOrEmpty(environment.Include))
            {
                var envToInclude = environment.Include.Split(',');

                foreach (var toInclude in envToInclude)
                {
                    var includeEnv = Serializer.Deserialize(Path.Combine(new FileInfo(environmentFile).Directory.FullName, toInclude));

                    // just add the variables which are not available in the requested environment
                    foreach (var variable in includeEnv.Variables)
                    {
                        if (environment.Variables.All(v => !v.Name.Equals(variable.Name, StringComparison.OrdinalIgnoreCase)))
                        {
                            environment.Variables.Add(variable);
                        }
                    }
                }
            }
            
            return environment;
        }

        public Environment GetEnvironment(string environmentName)
        {
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

        public IEnumerable<string> GetEnvironments(bool excludeShared = true)
        {
            var files = Directory.GetFiles(EnvironmentDirectory.ToString(), "*.xml");

            return files;
        }

        private DirectoryInfo FindEnvironmentFolder(string startFolder)
        {
            Log.Debug("Look for environment starting in " + startFolder);
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
                Log.Warn("Folder " + envFolder.FullName + " is missing!");
                throw new DirectoryNotFoundException(".powerdeploy folder not found with start folder " + startFolder);
            }

            Log.Debug("Using environment folder from " + envFolder.FullName);

            return envFolder;
        }
    }
}