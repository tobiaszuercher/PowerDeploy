using System;

using PowerDeploy.Server.ServiceModel;

using Raven.Client;

using ServiceStack;

namespace PowerDeploy.Server.Services
{
    public class ConfigurationService : Service
    {
        public IDocumentStore DocumentStore { get; set; }

        public ServerSettings Get(QueryServerSettings request)
        {
            ServerSettings config;

            using (var session = DocumentStore.OpenSession())
            {
                config = session.Load<ServerSettings>("ServerSettings/1");

                if (config == null)
                {
                    // provide some default values and store them
                    config = new ServerSettings()
                    {
                        Id = 1, 
                        GitExecutable = @"c:\Program Files (x86)\git\bin\git.exe",
                        NuGetServerUri = new Uri("http://localhost/nuggy/nuget"),
                        VersionControlSystem = VersionControlSystem.Git,
                    };

                    session.Store(config);
                    session.SaveChanges();
                }
            }

            return config;
        }

        public void Put(ServerSettings request)
        {
            if (request.Id == default(int))
            {
                request.Id = 1;
            }

            using (var session = DocumentStore.OpenSession())
            {
                var config = session.Load<ServerSettings>("ServerSettings/1");


            }
        }
    }
}