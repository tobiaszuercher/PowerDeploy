using Funq;

using ServiceStack.WebHost.Endpoints;

namespace PowerDeploy.DeploymentService
{
    public class DashboardAppHost : AppHostBase
    {
        public DashboardAppHost() : base("Environment Service", typeof(DashboardAppHost).Assembly)
        {
            
        }

        public override void Configure(Container container)
        {
            ////var dbConnectionFactory = new OrmLiteConnectionFactory(HttpContext.Current.Server.MapPath("~/App_Data/db.sql"), SqliteDialect.Provider);
            ////container.Register<IDbConnectionFactory>(dbConnectionFactory);
        }
    }
}