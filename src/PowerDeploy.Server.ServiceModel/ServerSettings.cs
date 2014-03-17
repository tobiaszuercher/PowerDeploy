using System;

using ServiceStack;

namespace PowerDeploy.Server.ServiceModel
{
    [Route("/configuration", Verbs = "GET")]
    public class QueryServerSettings : IReturn<ServerSettings>
    {
    }

    [Route("/configuration", Verbs = "PUT")]
    [Route("/configuration/{Id}", Verbs = "PUT")]
    public class ServerSettings
    {
        public int Id { get; set; }
        public string GitExecutable { get; set; }
        public Uri NuGetServerUri { get; set; }

        // in future those will be refactored to a project configuration
        public string EnvironmentsPath { get; set; }
        public string RepositoryUrl { get; set; }
        public string SourceControlWorkingDir { get; set; }
           
        public VersionControlSystem VersionControlSystem { get; set; }
    }

    public enum VersionControlSystem
    {
        Git,
        Tfs,
    }
}