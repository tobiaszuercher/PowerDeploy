using PowerDeploy.IISDeployService.Auth;
using PowerDeploy.IISDeployService.Contract;

using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

using System.Linq;

namespace PowerDeploy.IISDeployService
{
    public class ApiKeyAuthAttribute : RequestFilterAttribute
    {
        public IApiKeyValidator ApiKeyValidator { get; set; }

        public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
        {
            string apiKey;

            if (req.Headers.AllKeys.Contains("X-API-Key"))
            {
                apiKey = req.Headers["X-API-Key"];
            }
            else
            {
                // try to get it through the dto
                var apiKeyDto = requestDto as IHasApiKey;
                
                if (apiKeyDto != null)
                {
                    apiKey = apiKeyDto.ApiKey;
                }
                else
                {
                    throw HttpError.Unauthorized("No API-Key provided.");
                }
            }

            if (!ApiKeyValidator.Validate(apiKey))
            {
                throw HttpError.Unauthorized("API-Key invalid.");
            }
        }
    }
}