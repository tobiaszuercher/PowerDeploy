namespace PowerDeploy.IISDeployService.Auth
{
    public interface IApiKeyValidator
    {
        bool Validate(string apiKey);
    }
}