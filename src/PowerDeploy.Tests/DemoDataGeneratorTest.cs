using NUnit.Framework;
using PowerDeploy.Server.Model;
using Raven.Client.Document;

namespace PowerDeploy.Tests
{
    [TestFixture]
    public class DemoDataGenerator
    {
        [Test]
        [Ignore]
        public void Generate_Demo_Data()
        {
            var documentStore = new DocumentStore() { DefaultDatabase = "PowerDeploy", Url = "http://localhost:8080", }.Initialize();

            using (var session = documentStore.OpenSession())
            {
                session.ClearDocuments<Environment>();
                session.ClearDocuments<Package>();
                session.ClearDocuments<Deployment>();

                session.SaveChanges();
            }

            documentStore.CreateSzenario()
                .PublishPackage("RestAPI", "1.0.0")
                .PublishPackage("RestAPI", "1.0.1")
                .PublishPackage("RestAPI", "1.1.0")
                .PublishPackage("RestAPI", "2.0.0")
                .PublishPackage("WebApp", "1.0.0")
                .PublishPackage("WebApp", "1.1.0")
                .PublishPackage("WebApp", "2.0.0")
                .Deploy(DeploySzenario.Environment.Dev, "RestAPI", "1.0.0")
                .Deploy(DeploySzenario.Environment.Dev, "RestAPI", "1.0.1")
                .Deploy(DeploySzenario.Environment.Dev, "RestAPI", "1.1.0")
                .Deploy(DeploySzenario.Environment.Dev, "RestAPI", "2.0.0")
                .Deploy(DeploySzenario.Environment.Dev, "WebApp", "1.0.0")
                .Deploy(DeploySzenario.Environment.Dev, "WebApp", "1.1.0")
                .Deploy(DeploySzenario.Environment.Dev, "WebApp", "2.0.0")
                .Deploy(DeploySzenario.Environment.Test, "RestAPI", "1.0.0")
                .Deploy(DeploySzenario.Environment.Test, "RestAPI", "1.0.1")
                .Deploy(DeploySzenario.Environment.Test, "RestAPI", "1.1.0")
                .Deploy(DeploySzenario.Environment.Test, "RestAPI", "2.0.0")
                .Deploy(DeploySzenario.Environment.Test, "WebApp", "2.0.0")
                .Deploy(DeploySzenario.Environment.Dev, "WebApp", "2.0.0")
                .Deploy(DeploySzenario.Environment.Prod, "WebApp", "1.0.1")
                .Deploy(DeploySzenario.Environment.Prod, "WebApp", "1.1.0")
                .Play();


        }
    }
}
