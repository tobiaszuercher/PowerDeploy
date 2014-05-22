using System.IO;
using System.Linq;

using PowerDeploy.Server.Model;
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
                    var e1 = new Environment() { Id = 1, Name = "DEV", Description = "Development environment for the Dev's" };
                    var e2 = new Environment() { Id = 2, Name = "TEST", Description = "Dedicated environment for tester." };
                    var e3 = new Environment() { Id = 3, Name = "PROD", Description = "Production." };
                    var e4 = new Environment() { Id = 4, Name = "unittest", Description = "unit tests" };

                    session.Store(e1);
                    session.Store(e2);
                    session.Store(e3);
                    session.Store(e4);

                    session.SaveChanges();
                }

                if (session.Load<ServerSettings>("ServerSettings/1") == null)
                {
                    session.Store(new ServerSettings()
                    {
                        Id = 1, 
                        WorkDir = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), "PowerDeploy.UnitTests")
                    });

                    session.SaveChanges();
                }
            }
        }
    }
}
