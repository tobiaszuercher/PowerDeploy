using System.Runtime.Serialization;
using System.Xml.Serialization;

using PowerDeploy.IISDeployService.Contract;

namespace PowerDeploy.Core.Deploy
{
    [XmlRoot("package")]
    public class WebOptions
    {
        [XmlAttribute("appPoolName")]
        public string AppPoolName { get; set; }

        [XmlAttribute("appPoolUser")]
        public string AppPoolUser { get; set; }
        
        [XmlAttribute("appPoolPassword")]
        public string AppPoolPassword { get; set; }
        
        [XmlAttribute("websiteName")]
        public string WebsiteName { get; set; }
        
        [XmlAttribute("websitePhysicalPath")]
        public string WebsitePhysicalPath { get; set; }
        
        [XmlAttribute("appRoot")]
        public string AppRoot { get; set; }
        
        [XmlAttribute("appName")]
        public string AppName { get; set; }
        
        [XmlAttribute("appPhysicalPath")]
        public string AppPhysicalPath { get; set; }
        
        [XmlAttribute("websitePort")]
        public int WebsitePort { get; set; }
        
        [XmlAttribute("runtimeVersion")]
        public RuntimeVersion RuntimeVersion { get; set; }
        
        [XmlAttribute("overwrite")]
        public bool Overwrite { get; set; }

        [XmlAttribute("DeployService")]
        public string DeployService { get; set; }
    }
}