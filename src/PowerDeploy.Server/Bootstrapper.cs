using Funq;

using PowerDeploy.Core;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.Services;

using Raven.Client;

namespace PowerDeploy.Server
{
    public class Bootstrapper
    {
        public static void ConfigureDependencies(Container container, IDocumentStore documentStore)
        {
            container.Register<IDocumentStore>(documentStore);
            container.RegisterAutoWired<PackageProvider>();
            container.RegisterAutoWired<PackageService>();
            container.RegisterAutoWired<DeployService>();
            container.RegisterAutoWired<PhysicalFileSystem>();
            container.Register<IFileSystem>(new PhysicalFileSystem());
            container.Register<ServerSettings>(c => GetServerSettings(c.Resolve<IDocumentStore>())).ReusedWithin(ReuseScope.Request);
            container.RegisterAutoWired<NugetPackageDownloader>();
        }

        private static ServerSettings GetServerSettings(IDocumentStore documentStore)
        {
            using (var session = documentStore.OpenSession())
            {
                return session.Load<ServerSettings>("ServerSettings/1");
            }
        }
    }
}