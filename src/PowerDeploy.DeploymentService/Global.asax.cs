using System;
using System.Web;

namespace PowerDeploy.DeploymentService
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new DashboardAppHost().Init();
        }
    }
}