using ServiceStack.ServiceHost;

namespace PowerDeploy.IISDeployService.Contract
{
    [Api("Rollback the deployment.")]
    [Route("/deployments/rollback", Verbs = "POST")]
    public class RollbackDeployment : IReturnVoid
    {
        [ApiMember(Name = "WebsiteName", ParameterType = "query", Description = "The website name.", DataType = "string", IsRequired = true)]
        public string WebsiteName { get; set; }

        [ApiMember(Name = "AppRoot", ParameterType = "query", Description = "The path within the website.", DataType = "string", IsRequired = false)]
        public string AppRoot { get; set; }

        [ApiMember(Name = "AppName", ParameterType = "query", Description = "The name of the application.", DataType = "string", IsRequired = false)]
        public string AppName { get; set; }

        [ApiMember(Name = "RollbackTarget", ParameterType = "query", Description = "The rollback target", DataType = "string", IsRequired = false)]
        public string RollbackTarget { get; set; }
    }
}