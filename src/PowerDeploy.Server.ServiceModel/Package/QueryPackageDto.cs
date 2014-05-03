using System.Collections.Generic;

using ServiceStack;

namespace PowerDeploy.Server.ServiceModel.Package
{
    [Route("/packages", Verbs = "GET")]
    [Route("/packages/{NugetId}", Verbs = "GET")]
    public class QueryPackagesDto : IReturn<List<PackageDto>>
    {
        
    }
    
    [Route("/packages/{NugetId}/{Version}", Verbs = "GET")]
    public class QueryPackageDto : IReturn<PackageDto>
    {
        public string NugetId { get; set; }
        public string Version { get; set; }
    }
}