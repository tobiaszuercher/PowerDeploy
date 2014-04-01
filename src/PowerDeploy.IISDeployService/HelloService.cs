using PowerDeploy.IISDeployService.Contract;

using ServiceStack;

namespace PowerDeploy.IISDeployService
{
    public class HelloService : Service
    {
         public string Get(Hello request)
         {
             return "Hello {0}!".Fmt(request.Name);
         }
    }
}