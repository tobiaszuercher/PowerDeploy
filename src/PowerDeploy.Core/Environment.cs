using System.Collections.Generic;
using System.Linq;
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

        [XmlAttribute("include")]
        public string Include { get; set; }

        [XmlElement("variable")]
        public List<Variable> Variables { get; set; }

        [XmlIgnore]
        public Variable this[string name]
        {
            get { return Variables.FirstOrDefault(v => v.Name == name); }
        }
    }
}