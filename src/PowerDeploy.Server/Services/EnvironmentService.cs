using System.Linq;

using PowerDeploy.Server.ServiceModel;

using Raven.Client;

using ServiceStack;

namespace PowerDeploy.Server.Services
{
    public class EnvironmentService : Service
    {
        public IDocumentStore DocumentStore { get; set; }

         public object Get(QueryEnvironment request)
         {
             using (var session = DocumentStore.OpenSession())
             {
                 if (request.Id == default(int))
                 {
                     return session.Query<Environment>().ToList();
                 }

                 return session.Load<Environment>("Environment/" + request.Id);
             }
         }
    }
}