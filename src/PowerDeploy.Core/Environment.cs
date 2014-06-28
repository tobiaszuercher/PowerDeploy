using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

using PowerDeploy.Core.Cryptography;

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

        public void DecryptVariables(string aesKey)
        {
            foreach (var variable in this.Variables.Where(p => p.Encrypted))
            {
                variable.Value = AES.Decrypt(variable.Value, aesKey);
                variable.Encrypted = false;
            }
        }
    }
}