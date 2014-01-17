using System.Xml.Serialization;

namespace PowerDeploy.Core.Deploy
{
    [XmlRoot("package")]
    public class XCopyOptions
    {
        [XmlAttribute("environment")]
        public string Environment { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlElement("destination")]
        public string Destination { get; set; }
    }
}