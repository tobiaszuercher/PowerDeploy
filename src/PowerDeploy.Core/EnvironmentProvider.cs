using System;
using System.IO;
using System.Linq;

using PowerDeploy.Core.Extensions;

namespace PowerDeploy.Core
{
    // Todo: add proper tests
    public class EnvironmentProvider
    {
        public XmlEnvironmentParser Parser { get; set; }

        public EnvironmentLocator Locator { get; set; }

        public EnvironmentProvider()
        {
            Parser = new XmlEnvironmentParser();
        }

        public Environment GetEnvironmentFromFile(string path)
        {
            return Parser.GetEnvironmentFromFile(path);
        }

        public Environment GetEnvironment(string startFolder, string environmentName)
        {
            var dirInfo = new DirectoryInfo(startFolder);
            var root = Directory.GetDirectoryRoot(startFolder);

            while (dirInfo.FullName != root)
            {
                dirInfo = Directory.GetParent(dirInfo.FullName);

                if (dirInfo.GetDirectories(".powerdeploy").Any())
                {
                    break;
                }
            }

            dirInfo = new DirectoryInfo(Path.Combine(dirInfo.FullName, ".powerdeploy"));

            if (dirInfo.Exists)
            {
                return GetEnvironmentFromFile(Path.Combine(dirInfo.FullName, "{0}.xml".Fmt(environmentName)));
            }
            
            throw new InvalidOperationException(".powerdeploy folder not found");
        }
    }
}