using PowerDeploy.Server.ServiceModel;

using ServiceStack;

namespace PowerDeploy.Server.Services
{
    public class DeploymentQueueService : Service
    {
        public TriggerDeploymentMessageQueueResponse Post(TriggerDeploymentMessageQueue request)
        {
            return new TriggerDeploymentMessageQueueResponse() { Success = true };
        }
    }
}