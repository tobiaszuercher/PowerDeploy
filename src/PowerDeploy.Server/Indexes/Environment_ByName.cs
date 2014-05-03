using System.Linq;

using PowerDeploy.Server.ServiceModel;

using Raven.Client.Indexes;

namespace PowerDeploy.Server.Indexes
{
    public class Environment_ByName : AbstractIndexCreationTask<Environment>
    {
        public Environment_ByName()
        {
            Map = environments => from environment in environments select new { environment.Name };
        }
    }
}