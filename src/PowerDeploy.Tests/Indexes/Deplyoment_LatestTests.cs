using System.Linq;

using NUnit.Framework;

using PowerDeploy.Server.Indexes;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Package;
using PowerDeploy.Tests;
using Raven.Tests.Helpers;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Powerdeploy.Server.Tests.Indexes
{
    [TestFixture]
    public class Deplyoment_LatestTests : RavenTestBase
    {
        [Test]
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

                    Assert.AreEqual("1.0.0.1", package.Package.Version);
                    Assert.AreEqual("ConsoleApp", package.Package.NugetId);
                    Assert.AreEqual("packages/ConsoleApp/1.0.0.1", package.Package.Id);
                    Assert.AreEqual(2, package.Deployments);
                }
            }
        }

        [Test]
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

                    ////Assert.AreEqual("1.0.0.0", package.PackageVersion);
                    Assert.AreEqual("ConsoleApp", package.Package.NugetId);
                    Assert.AreEqual("packages/ConsoleApp/1.0.0.0", package.Package.Id);
                    Assert.AreEqual(2, package.Deployments);
                }
            }
        }

        [Test]
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

                    //Assert.AreEqual("1.0.0.1", package.PackageVersion);
                    Assert.AreEqual("ConsoleApp", package.Package.NugetId);
                    Assert.AreEqual("packages/ConsoleApp/1.0.0.1", package.Package.Id);
                    Assert.AreEqual(1, package.Deployments);
                }
            }
        }

        [Test]
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

                    WaitForUserToContinueTheTest(store);

                    Assert.AreEqual("packages/ConsoleApp/1.0.0.1", consoleApp.Package.Id);
                    Assert.AreEqual(2, consoleApp.Deployments);

                    Assert.AreEqual("packages/WebApp/1.0.0.0", webApp.Package.Id);
                    Assert.AreEqual(1, webApp.Deployments);
                }
            }
        }

        [Test]
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

                    Assert.AreEqual("packages/ConsoleApp/1.1.0.0", dev.Package.Id);
                    Assert.AreEqual(4, dev.Deployments);

                    Assert.AreEqual("packages/ConsoleApp/1.0.1.0", test.Package.Id);
                    Assert.AreEqual(3, test.Deployments);

                    Assert.AreEqual("packages/ConsoleApp/1.0.0.1", prod.Package.Id);
                    Assert.AreEqual(2, prod.Deployments);
                }
            }
        }
    }
}