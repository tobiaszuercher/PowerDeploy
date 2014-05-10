using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Server.Indexes;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Package;

using Raven.Tests.Helpers;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Powerdeploy.Server.Tests.Indexes
{
    [TestClass]
    public class Deplyoment_LatestTests : RavenTestBase
    {
        [TestMethod]
        public void Deploy_Old_Then_Newer_Shows_Latest()
        {
            using (var store = NewDocumentStore())
            {
                new Deployment_Latest().Execute(store);

                store.CreateSzenario()
                    .PublishPackage("ConsoleApp", "1.0.0.0")
                    .PublishPackage("ConsoleApp", "1.0.0.1")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.0")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.1")
                    .Play();

                using (var session = store.OpenSession())
                {
                    var results = session.Query<Deployment_Latest.ReducedResult, Deployment_Latest>();

                    var package = results.FirstOrDefault();

                    Assert.AreEqual("1.0.0.1", package.PackageVersion);
                    Assert.AreEqual("ConsoleApp", package.NugetId);
                    Assert.AreEqual(new PackageDto("ConsoleApp", "1.0.0.1").Id, package.PackageId);
                    Assert.AreEqual(2, package.Deployments);
                }
            }
        }

        [TestMethod]
        public void Deploy_New_Then_Old_Shows_Old()
        {
            using (var store = NewDocumentStore())
            {
                new Deployment_Latest().Execute(store);

                store.CreateSzenario()
                    .PublishPackage("ConsoleApp", "1.0.0.0")
                    .PublishPackage("ConsoleApp", "1.0.0.1")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.1")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.0")
                    .Play();


                using (var session = store.OpenSession())
                {
                    var results = session.Query<Deployment_Latest.ReducedResult, Deployment_Latest>();

                    var package = results.FirstOrDefault();

                    Assert.AreEqual("1.0.0.0", package.PackageVersion);
                    Assert.AreEqual("ConsoleApp", package.NugetId);
                    Assert.AreEqual(new PackageDto("ConsoleApp", "1.0.0.0").Id, package.PackageId);
                    Assert.AreEqual(2, package.Deployments);
                }
            }
        }

        [TestMethod]
        public void Publish_Different_Packages_One_Deployment_Shows_Latest()
        {
            using (var store = NewDocumentStore())
            {
                new Deployment_Latest().Execute(store);

                store.CreateSzenario()
                    .PublishPackage("ConsoleApp", "1.0.0.0")
                    .PublishPackage("ConsoleApp", "1.0.0.1")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.1")
                    .Play();

                using (var session = store.OpenSession())
                {
                    var results = session.Query<Deployment_Latest.ReducedResult, Deployment_Latest>();

                    var package = results.FirstOrDefault();

                    Assert.AreEqual("1.0.0.1", package.PackageVersion);
                    Assert.AreEqual("ConsoleApp", package.NugetId);
                    Assert.AreEqual(new PackageDto("ConsoleApp", "1.0.0.1").Id, package.PackageId);
                    Assert.AreEqual(1, package.Deployments);
                }
            }
        }

        [TestMethod]
        public void Deploy_Different_Packages_Shows_Correct_Latest()
        {
            using (var store = NewDocumentStore())
            {
                new Deployment_Latest().Execute(store);

                store.CreateSzenario()
                    .PublishPackage("ConsoleApp", "1.0.0.0")
                    .PublishPackage("ConsoleApp", "1.0.0.1")
                    .PublishPackage("WebApp", "1.0.0.0")
                    .PublishPackage("WebApp", "2.0.0.0")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.0")
                    .Deploy(DeploySzenario.Environment.Dev, "WebApp", "1.0.0.0")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.1")
                    .Play();

                using (var session = store.OpenSession())
                {
                    var results = session.Query<Deployment_Latest.ReducedResult, Deployment_Latest>();
                    var consoleApp = results.First();
                    var webApp = results.Skip(1).First();

                    Assert.AreEqual(new PackageDto("ConsoleApp", "1.0.0.1").Id, consoleApp.PackageId);
                    Assert.AreEqual(2, consoleApp.Deployments);

                    Assert.AreEqual(new PackageDto("WebApp", "1.0.0.0").Id, webApp.PackageId);
                    Assert.AreEqual(1, webApp.Deployments);
                }
            }
        }

        [TestMethod]
        public void Deploy_Different_Packages_To_Different_Environments_Shows_Correct_Latest()
        {
            using (var store = NewDocumentStore())
            {
                new Deployment_Latest().Execute(store);

                store.CreateSzenario()
                    .PublishPackage("ConsoleApp", "1.0.0.0")
                    .PublishPackage("ConsoleApp", "1.0.0.1")
                    .PublishPackage("ConsoleApp", "1.0.1.0")
                    .PublishPackage("ConsoleApp", "1.1.0.0")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.0")
                    .Deploy(DeploySzenario.Environment.Test, "ConsoleApp", "1.0.0.0")
                    .Deploy(DeploySzenario.Environment.Prod, "ConsoleApp", "1.0.0.0")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.1")
                    .Deploy(DeploySzenario.Environment.Test, "ConsoleApp", "1.0.0.1")
                    .Deploy(DeploySzenario.Environment.Prod, "ConsoleApp", "1.0.0.1")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.1.0")
                    .Deploy(DeploySzenario.Environment.Test, "ConsoleApp", "1.0.1.0")
                    .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.1.0.0")
                    .Play();

                using (var session = store.OpenSession())
                {
                    var results = session.Query<Deployment_Latest.ReducedResult, Deployment_Latest>();

                    var dev = results.First(d => d.EnvironmentName == DeploySzenario.Environment.Dev.ToString());
                    var test = results.First(d => d.EnvironmentName == DeploySzenario.Environment.Test.ToString());
                    var prod = results.First(d => d.EnvironmentName == DeploySzenario.Environment.Prod.ToString());

                    Assert.AreEqual(new PackageDto("ConsoleApp", "1.1.0.0").Id, dev.PackageId);
                    Assert.AreEqual(4, dev.Deployments);

                    Assert.AreEqual(new PackageDto("ConsoleApp", "1.0.1.0").Id, test.PackageId);
                    Assert.AreEqual(3, test.Deployments);

                    Assert.AreEqual(new PackageDto("ConsoleApp", "1.0.0.1").Id, prod.PackageId);
                    Assert.AreEqual(2, prod.Deployments);
                }
            }
        }
    }
}