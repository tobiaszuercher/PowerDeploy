namespace PowerDeploy.Server.ServiceModel
{
    public class SynchronizationSummary
    {
        public int AddedPackages { get; set; }
        public int PackagesInNuget { get; set; }
        public int PackagesInPowerDeploy { get; set; }
    }
}