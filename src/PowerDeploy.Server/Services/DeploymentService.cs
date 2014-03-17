using PowerDeploy.Server.ServiceModel;

using ServiceStack;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;

namespace PowerDeploy.Server.Services
{
    public class DeploymentService : Service
    {
        public object Post(TriggerDeployment request)
        {
            var redisFactory = new PooledRedisClientManager("localhost:6379");
            var mqHost = new RedisMqServer(redisFactory);

            var client = mqHost.CreateMessageQueueClient();

            ////var m = client.CreateMessage<Hello>(new Hello());

            client.Publish(request);

            return null;
        }
    }
}