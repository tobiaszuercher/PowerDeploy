using System.Collections.Generic;

using PowerDeploy.Server.ServiceModel.Package;

namespace PowerDeploy.Server.ServiceModel
{
    public class Release
    {
        public string Name { get; set; }
        public List<PackageWithVersionDto> Packages { get; set; }
    }
}