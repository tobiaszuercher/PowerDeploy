using Funq;

using PowerDeploy.IISDeployService.Auth;

using ServiceStack;
using ServiceStack.Api.Swagger;
using ServiceStack.Configuration;

namespace PowerDeploy.IISDeployService
{
    public class AppHost : AppHostBase
    {
        public AppHost()
            : base("IIS Deploy Service", typeof(AppHost).Assembly)
        {

        }

        public override void Configure(Container container)
        {
            ////var dbConnectionFactory = new OrmLiteConnectionFactory(HttpContext.Current.Server.MapPath("~/App_Data/db.sql"), SqliteDialect.Provider);
            ////container.Register<IDbConnectionFactory>(dbConnectionFactory);
            
            IAppSettings appSettings = new AppSettings();
            
            Plugins.Add(new SwaggerFeature());
            //Plugins.Add(new RequestLogsFeature() { RequiredRoles = null });

            container.Register(appSettings);
            container.RegisterAutoWired<IISManager>();
            container.RegisterAutoWired<AppSettingsApiKeyValidator>();
        }
    }
}