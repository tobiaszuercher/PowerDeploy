using System.Collections.Generic;
using System.Xml.Serialization;

namespace PowerDeploy.Core
{
    [XmlRoot("environment", Namespace = "")]
    public class Environment
    {
        [XmlAttribute("name")]
        public string Name { get; set; }     

        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlElement("variable")]
        public List<Variable> Variables { get; set; }
    }
}