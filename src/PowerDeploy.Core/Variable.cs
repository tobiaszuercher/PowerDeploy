using System.Xml.Serialization;

namespace PowerDeploy.Core
{
    public class Variable
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }

        [XmlAttribute("encrypted")]
        public bool Encrypted { get; set; }

        [XmlAttribute("do-encrypt")]
        public bool DoEncrypt { get; set; }
    }
}