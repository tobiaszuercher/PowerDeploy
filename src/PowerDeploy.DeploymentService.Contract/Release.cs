using System.Collections.Generic;

namespace PowerDeploy.DeploymentService.Contract
{
    public class Release
    {
        public string Name { get; set; }
        public List<Package> Packages { get; set; }
    }
}