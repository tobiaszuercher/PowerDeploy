using System;

using Funq;

using ServiceStack.Logging;
using ServiceStack.Logging.Support.Logging;

using ServiceStack.WebHost.Endpoints;

namespace MovieFavorites.Services
{
    using ServiceStack.Api.Swagger;
    using ServiceStack.MiniProfiler;

    public class Global : System.Web.HttpApplication
    {
        public class MovieFavoritesAppHost : AppHostBase
        {
            // Tell Service Stack the name of your application and where to find your web services
            public MovieFavoritesAppHost() : base("Movie Favorites Demo Services", typeof(MovieService).Assembly) { }

            public override void Configure(Container container)
            {
                SetConfig(new EndpointHostConfig { LogFactory = new DebugLogFactory() });
                Plugins.Add(new SwaggerFeature());
                //register any dependencies your services use, e.g:
                //container.Register<ICacheClient>(new MemoryCacheClient());
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            new MovieFavoritesAppHost().Init();

            LogManager.LogFactory = new DebugLogFactory(); 
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (Request.IsLocal)
            {
                Profiler.Start();
            }
        }

        protected void Application_EndRequest(object sender, EventArgs e)
        {
            Profiler.Stop();
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}