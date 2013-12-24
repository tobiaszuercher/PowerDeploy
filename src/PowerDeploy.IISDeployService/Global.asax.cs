using System;
using System.Web;

namespace PowerDeploy.IISDeployService
{
    public class Global : HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            new AppHost().Init();
        }
    }
}