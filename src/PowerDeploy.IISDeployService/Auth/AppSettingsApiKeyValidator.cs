using ServiceStack.Configuration;

namespace PowerDeploy.IISDeployService.Auth
{
    /// <summary>
    /// Validates the API-Key against the appsetting.
    /// </summary>
    public class AppSettingsApiKeyValidator : IApiKeyValidator
    {
        public IAppSettings Settings { get; set; }
        
        public bool Validate(string apiKey)
        {
            return Settings.GetList("auth.api-keys").Contains(apiKey.Trim());
        }
    }
}