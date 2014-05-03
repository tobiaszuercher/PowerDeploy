using System.Linq;

using PowerDeploy.Server.ServiceModel;

using Raven.Client;

namespace PowerDeploy.Server
{
    public class DataInitializer
    {
        public static void InitializerWithDefaultValuesIfEmpty(IDocumentStore documentStore)
        {
            using (var session = documentStore.OpenSession())
            {
                if (!session.Query<EnvironmentDto>().Any())
                {
                    var e1 = new EnvironmentDto() { Id = 1, Name = "DEV", Description = "Development environment for the Dev's" };
                    var e2 = new EnvironmentDto() { Id = 2, Name = "TEST", Description = "Dedicated environment for tester." };
                    var e3 = new EnvironmentDto() { Id = 3, Name = "PROD", Description = "Production." };

                    session.Store(e1);
                    session.Store(e2);
                    session.Store(e3);

                    session.SaveChanges();
                }
            }
        }
    }
}
