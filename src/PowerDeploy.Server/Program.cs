using System;

using Funq;

using PowerDeploy.Server.Indexes;

using Raven.Client.Document;
using Raven.Client.Indexes;

using ServiceStack;
using ServiceStack.Api.Swagger;

namespace PowerDeploy.Server
{
    public class ServerAppHost : AppHostHttpListenerBase
    {
        public ServerAppHost() : base("Powerdeploy Server", typeof(ServerAppHost).Assembly) { }

        public override void Configure(Container container)
        {
            Plugins.Add(new SwaggerFeature());

            var documentStore = new DocumentStore() { DefaultDatabase = "PowerDeploy", Url = "http://localhost:8080", }.Initialize();

            IndexCreation.CreateIndexes(typeof(Deployment_Latest).Assembly, documentStore);

            Bootstrapper.ConfigureDependencies(container, documentStore);

            DataInitializer.InitializerWithDefaultValuesIfEmpty(documentStore);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var serverAppHost = new ServerAppHost();
            serverAppHost.Init();
            serverAppHost.Start("http://localhost:81/"); //Starts HttpListener listening on 81
            Console.WriteLine("PowerDeploy server started on localhost:81");
            Console.ReadLine(); //Block the server from exiting (i.e. if running inside Console App)
        }
    }
}
