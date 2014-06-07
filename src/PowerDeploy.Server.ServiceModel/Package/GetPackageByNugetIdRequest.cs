using System.Collections.Generic;
using ServiceStack;

namespace PowerDeploy.Server.ServiceModel.Package
{
    [Route("/packages/{NugetId}", "GET")]
    public class GetPackageByNugetIdRequest : IReturn<PackageWithVersionDto>
    {
        [ApiMember(Name = "NugetId", ParameterType = "path", IsRequired = false)]
        public string NugetId { get; set; }
    }

    [Route("/packages", "GET")]
    public class GetPackagesRequest : IReturn<List<PackageOverviewDto>>
    {
    }
}