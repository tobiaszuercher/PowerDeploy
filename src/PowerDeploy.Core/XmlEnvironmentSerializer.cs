using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PowerDeploy.Core
{
    public class XmlEnvironmentSerializer : IEnvironmentSerializer
    {
        private readonly IFileSystem _fileSystem;

        public XmlEnvironmentSerializer()
            : this(new PhysicalFileSystem())
        {
        }
        
        public XmlEnvironmentSerializer(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public Environment Deserialize(string file)
        {
            var xml = _fileSystem.ReadFile(file);

            var serializer = new XmlSerializer(typeof(Environment));
            var environment = serializer.Deserialize(new XmlTextReader(new StringReader(xml))) as Environment;
            
            // todo: add error handling for failed deserializing
            environment.Variables.Add(new Variable() { Name = "env", Value = environment.Name.ToUpper(CultureInfo.InvariantCulture) });

            return environment;
        }
    }
}