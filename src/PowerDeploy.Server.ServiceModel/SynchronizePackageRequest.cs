using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    [Route("/packages/sync")]
    public class SynchronizePackageRequest : IReturn<SynchronizePackageResponse>
    {
    }

    public class SynchronizePackageResponse
    {
        public int AddedPackages { get; set; }
        public int TotalPackages { get; set; }
    }
}