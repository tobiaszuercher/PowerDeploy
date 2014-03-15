using Funq;

using PowerDeploy.DeploymentService.Contract;

using ServiceStack;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;

namespace PowerDeploy.DeploymentService
{
    public class DashboardAppHost : AppHostHttpListenerBase
    {
        public DashboardAppHost() : base("Environment Service", typeof(DashboardAppHost).Assembly)
        {
            
        }

        public override void Configure(Container container)
        {
            var redisFactory = new PooledRedisClientManager("localhost:6379");
            container.Register<IRedisClientsManager>(redisFactory); // req. to log exceptions in redis
            var mqHost = new RedisMqServer(redisFactory);

            
            mqHost.RegisterHandler<TriggerDeployment>(m => new TriggerDeploymentMessageQueue().PopulateWith(m));
        }
    }
}