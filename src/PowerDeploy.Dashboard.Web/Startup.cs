using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PowerDeploy.Dashboard.Web.Startup))]
namespace PowerDeploy.Dashboard.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
