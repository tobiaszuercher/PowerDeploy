using System;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PowerDeploy.Core
{
    public class XmlEnvironmentParser : IEnviornmentProvider
    {
        private readonly IFileSystem _fileSystem;

        public string Location { get; private set; }

        public XmlEnvironmentParser(string location)
            : this(new PhysicalFileSystem(), location)
        {
        }

        public XmlEnvironmentParser(IFileSystem fileSystem, string location)
        {
            Location = location;
            _fileSystem = fileSystem;
        }

        public Environment GetEnvironment(string environmentName)
        {
            if (!_fileSystem.DirectoryExists(Location))
            {
                throw new InvalidOperationException("The given environment location doesn't exist.");
            }

            return GetEnvironmentFromFile(Path.Combine(Location, environmentName + ".xml"));
        }

        public Environment GetEnvironmentFromFile(string file)
        {
            var xml = _fileSystem.ReadFile(file);

            var serializer = new XmlSerializer(typeof(Environment));
            var environment = serializer.Deserialize(new XmlTextReader(new StringReader(xml))) as Environment;
            environment.Variables.Add(new Variable() { Name = "env", Value = environment.Name.ToUpper(CultureInfo.InvariantCulture) });

            return environment;
        }
    }
}