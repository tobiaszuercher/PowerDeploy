using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    [Route("/package/sync", Verbs = "GET,POST", Summary = "Syncs packages from the nuget server to the PowerDeploy server.")]
    public class SynchronizePackageRequest : IReturn<SynchronizePackageResponse>
    {
    }

    public class SynchronizePackageResponse
    {
        public int AddedPackages { get; set; }
        public int TotalPackages { get; set; }
    }
}