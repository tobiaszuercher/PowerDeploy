using System.Collections.Generic;

using ServiceStack;

namespace PowerDeploy.IISDeployService.Contract
{
    [Api("List deployed packages.")]
    [Route("/deployments", Verbs = "GET")]
    [Route("/deployments/{WebsiteName}", Verbs = "GET")]
    public class QueryDeployments : IReturn<List<DeploymentInfo>>
    {
        [ApiMember(Name = "WebsiteName", ParameterType = "query", Description = "The website name.", DataType = "string", IsRequired = true)]
        public string WebsiteName { get; set; }

        [ApiMember(Name = "AppRoot", ParameterType = "query", Description = "The path within the website.", DataType = "string", IsRequired = false)]
        public string AppRoot { get; set; }

        [ApiMember(Name = "AppName", ParameterType = "query", Description = "The name of the application.", DataType = "string", IsRequired = false)]
        public string AppName { get; set; }
    }

    public class DeploymentInfo
    {
        public string Name { get; set; }
    }
}
