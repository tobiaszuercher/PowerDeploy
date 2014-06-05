using System.Collections.Generic;
using ServiceStack;

namespace PowerDeploy.Server.ServiceModel.Package
{
    [Route("/packages/{NugetId}", "GET")]
    public class GetPackageByIdRequest : IReturn<PackageDto>
    {
        [ApiMember(Name = "NugetId", ParameterType = "path", IsRequired = false)]
        public string NugetId { get; set; }
    }

    [Route("/packages", "GET")]
    public class GetPackageRequest : IReturn<List<PackageOverviewDto>>
    {
    }
}