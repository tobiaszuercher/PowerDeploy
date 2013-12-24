using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PowerDeploy.Core
{
    public class XmlEnvironmentProvider : IEnviornmentProvider
    {
        private readonly IFileSystem _fileSystem;

        public string Location { get; private set; }

        public XmlEnvironmentProvider(string location)
            : this(new PhysicalFileSystem(), location)
        {
        }

        public XmlEnvironmentProvider(IFileSystem fileSystem, string location)
        {
            Location = location;
            _fileSystem = fileSystem;
        }

        public Environment GetVariables(string environmentName)
        {
            var xml = _fileSystem.ReadFile(Path.Combine(Location, environmentName + ".xml"));

            var serializer = new XmlSerializer(typeof(Environment));
            var environment = serializer.Deserialize(new XmlTextReader(new StringReader(xml))) as Environment;

            return environment;
        }
    }
}