using System;

using Funq;

using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;

using Raven.Client.Document;

using ServiceStack;
using ServiceStack.Api.Swagger;
using ServiceStack.Messaging;
using ServiceStack.Messaging.Redis;
using ServiceStack.Redis;


namespace PowerDeploy.Server
{
    public class ServerAppHost : AppHostHttpListenerBase
    {
        public ServerAppHost() : base("Powerdeploy Server", typeof(ServerAppHost).Assembly) { }

        public override void Configure(Container container)
        {
            Plugins.Add(new SwaggerFeature());

            var documentStore = new DocumentStore() { DefaultDatabase = "PowerDeploy", Url = "http://localhost:8080", }.Initialize();

            container.Register(documentStore);
            container.RegisterAutoWired<PackageProvider>();

            var redisFactory = new PooledRedisClientManager("localhost:6379");
            container.Register<IRedisClientsManager>(redisFactory); // req. to log exceptions in redis
            var mqHost = new RedisMqServer(redisFactory);

            mqHost.RegisterHandler<TriggerDeploymentMessageQueueResponse>(m =>
            {
                Console.WriteLine("Deployment {0} done".Fmt(m.GetBody()));
                return null;
            });

            mqHost.RegisterHandler<TriggerDeployment>(
                m =>
                {
                    var mqMessage = new Message<TriggerDeploymentMessageQueue>(new TriggerDeploymentMessageQueue().PopulateWith(m.GetBody()));
                    ServiceController.ExecuteMessage(mqMessage);

                    return null;
                });

            mqHost.Start(); //Starts listening for messages
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
