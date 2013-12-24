using System.Collections.Generic;

using Funq;

using PowerDeploy.IISDeployService.Auth;

using ServiceStack.Configuration;
using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;
using ServiceStack.ServiceInterface.Admin;
using ServiceStack.WebHost.Endpoints;

namespace PowerDeploy.IISDeployService.Tests
{
    /// <summary>
    /// AppHost for integration tests.
    /// </summary>
    public class SelfAppHost : AppHostHttpListenerBase
    {
        public SelfAppHost()
            : base("Integration Tests", typeof(AppHost).Assembly)
        {
        }

        public override void Configure(Container container)
        {
            LogManager.LogFactory = new ConsoleLogFactory();

            IResourceManager appSettings = new DictionarySettings(
                new Dictionary<string, string>
                    {
                        { "backup.history", "5" }, 
                        { "auth.api-keys", "1337"}
                    });

            container.Register(c => appSettings);
            container.RegisterAutoWired<IISManager>();
            container.RegisterAs<AppSettingsApiKeyValidator, IApiKeyValidator>();

            Plugins.Add(new RequestLogsFeature { RequiredRoles = null });
        }
    }
}