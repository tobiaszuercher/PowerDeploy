﻿using System;
using System.IO;
using System.Net;

using NUnit.Framework;
using Microsoft.Web.Administration;

using PowerDeploy.IISDeployService.Contract;

using ServiceStack;
using ServiceStack.Text;

using System.Linq;

namespace PowerDeploy.IISDeployService.Tests.Integration
{
    [TestFixture]
    public class DeploymentTests
    {
        private SelfAppHost _appHost;

        [TestFixtureSetUp]
        public void AppHostSetup()
        {
            _appHost = new SelfAppHost();
            _appHost.Init();
            _appHost.Start(Config.ListenOn);
        }

        [TestFixtureTearDown]
        public void AppHostShutdown()
        {
            _appHost.Stop();
            _appHost.Dispose();
        }

        [Test]
        [Category("Integration")]
        public void Trigger_Simple_Deployment_And_Check_AppPool_And_Website_Test()
        {
            var client = GetClient();

            var request = new TriggerDeployment()
            {
                AppPoolName = "ZZZ_Integration_PoolName",
                AppPoolUser = "JackBauer",
                AppPoolPassword = "topsecret",
                WebsiteName = "ZZZ_Integration_Website_Simple",
                AppRoot = "/",
                PackageId = "IntegrationTest",
                PackageVersion = "1.3.3.7",
                WebsitePhysicalPath = @"C:\temp\integrationtests",
                RuntimeVersion = RuntimeVersion.Version40,
            };

            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();

            using (var manager = new ServerManager())
            {
                Assert.IsNotNull(manager.ApplicationPools[request.AppPoolName]);
                Assert.AreEqual(request.AppPoolName, manager.ApplicationPools[request.AppPoolName].Name);
                Assert.AreEqual(request.AppPoolUser, manager.ApplicationPools[request.AppPoolName].ProcessModel.UserName);
                Assert.AreEqual(request.AppPoolPassword, manager.ApplicationPools[request.AppPoolName].ProcessModel.Password);

                Assert.IsNotNull(manager.Sites[request.WebsiteName]);
                Assert.AreEqual(Path.Combine(request.WebsitePhysicalPath, "{0}_v{1}".Fmt(request.PackageId, request.PackageVersion)), manager.Sites[request.WebsiteName].Applications["/"].VirtualDirectories["/"].PhysicalPath);
            }
            
            IISManagerTestBuddy.DeleteAppPool(request.AppPoolName);
            IISManagerTestBuddy.DeleteWebsite(request.WebsiteName);
        }

        [Test]
        [Category("Integration")]
        public void Trigger_Deployment_With_App_Virtual_Dir_And_Check_AppPool_And_Website_Test()
        {
            var client = GetClient();

            var request = new TriggerDeployment()
            {
                AppPoolName = "ZZZ_Integration_PoolName",
                AppPoolUser = "JackBauer",
                AppPoolPassword = "topsecret",
                WebsiteName = "ZZZ_Integration_Website_VDIR",
                AppRoot = "/sub1/sub2",
                PackageId = "IntegrationTest",
                PackageVersion = "1.3.3.7",
                WebsitePhysicalPath = @"C:\temp\www",
                WebsitePort = 8000,
                RuntimeVersion = RuntimeVersion.Version40,
                AppName = "App",
                AppPhysicalPath = @"c:\temp\int-app",
            };

            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();

            using (var manager = new ServerManager())
            {
                Assert.IsNotNull(manager.ApplicationPools[request.AppPoolName]);
                Assert.AreEqual(request.AppPoolName, manager.ApplicationPools[request.AppPoolName].Name);
                Assert.AreEqual(request.AppPoolUser, manager.ApplicationPools[request.AppPoolName].ProcessModel.UserName);
                Assert.AreEqual(request.AppPoolPassword, manager.ApplicationPools[request.AppPoolName].ProcessModel.Password);

                Assert.IsNotNull(manager.Sites[request.WebsiteName]);
                Assert.AreEqual(@"{0}\{1}_v{2}".Fmt(request.AppPhysicalPath, request.PackageId, request.PackageVersion), manager.Sites[request.WebsiteName].Applications["{0}/{1}".Fmt(request.AppRoot.TrimEnd('/'), request.AppName)].VirtualDirectories["/"].PhysicalPath);
            }
        }

        [Test]
        [Category("Integration")]
        public void List_Deployed_Folders_For_a_Website_Test()
         {
            var client = GetClient();

            const string version1 = "1.3.3.7";
            const string version2 = "1.3.3.8";

            var request = new TriggerDeployment()
            {
                AppPoolName = "ZZZ_Integration_PoolName",
                AppPoolUser = "JackBauer",
                AppPoolPassword = "topsecret",
                WebsiteName = "ZZZ_Integration_Website",
                AppRoot = "/",
                PackageId = "IntegrationTest",
                PackageVersion = version1,
                WebsitePhysicalPath = @"C:\temp\test",
                RuntimeVersion = RuntimeVersion.Version40,
            };

            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();
            request.PackageVersion = version2;
            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();

            var response = client.Get(new QueryDeployments() { WebsiteName = request.WebsiteName });

            response.PrintDump();

            Assert.IsTrue(response.Any(r => r.Name.Contains(version1)));
            Assert.IsTrue(response.Any(r => r.Name.Contains(version2)));
        }

        [Test]
        [Category("Integration")]
        public void List_Deployed_Folders_For_a_Website_with_VDIR_Test()
        {
            var client = GetClient();

            const string version1 = "1.3.3.7";
            const string version2 = "1.3.3.8";

            var request = new TriggerDeployment()
            {
                AppPoolName = "ZZZ_Integration_PoolName",
                AppPoolUser = "JackBauer",
                AppPoolPassword = "topsecret",
                WebsiteName = "ZZZ_Integration_Website_VDIR",
                AppRoot = "/sub1/sub2",
                PackageId = "IntegrationTest",
                PackageVersion = version1,
                WebsitePhysicalPath = @"C:\temp\www",
                WebsitePort = 8000,
                RuntimeVersion = RuntimeVersion.Version40,
                AppName = "App",
                AppPhysicalPath = @"c:\temp\int-app",
            };

            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();
            request.PackageVersion = version2;
            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();

            var response = client.Get(new QueryDeployments() { WebsiteName = request.WebsiteName, AppName = request.AppName, AppRoot = request.AppRoot });

            response.PrintDump();

            Assert.IsTrue(response.Any(r => r.Name.Contains("{0}_v{1}".Fmt(request.PackageId, version1))));
            Assert.IsTrue(response.Any(r => r.Name.Contains("{0}_v{1}".Fmt(request.PackageId, version2))));
        }

        [Test]
        [Category("Integration")]
        public void Rollback_Website_Test()
        {
            var client = GetClient();

            var request = new TriggerDeployment()
            {
                AppPoolName = "ZZZ_Integration_PoolName",
                AppPoolUser = "JackBauer",
                AppPoolPassword = "topsecret",
                WebsiteName = "ZZZ_Integration_Website_Simple",
                AppRoot = "/",
                PackageId = "IntegrationTest",
                PackageVersion = "1.3.3.7",
                WebsitePhysicalPath = @"C:\temp\integrationtests",
                RuntimeVersion = RuntimeVersion.Version40,
                WebsitePort = 2000,
            };

            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();
            request.PackageVersion = "1.3.3.8";
            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();

            var rollbackRequest = new RollbackDeployment() { WebsiteName = request.WebsiteName, RollbackTarget = "{0}_v{1}".Fmt(request.PackageId, "1.3.3.7") };

            client.Post(rollbackRequest);

            IISManagerTestBuddy.AssertWebsite(request.WebsiteName, Path.Combine(request.WebsitePhysicalPath, rollbackRequest.RollbackTarget), request.WebsitePort, request.AppPoolName);
            IISManagerTestBuddy.DeleteAppPool(request.AppPoolName);
            IISManagerTestBuddy.DeleteWebsite(request.WebsiteName);
        }

        [Test]
        [Category("Integration")]
        public void Rollback_Website_with_VDIR_Test()
        {
            var client = GetClient();

            const string version1 = "1.3.3.7";
            const string version2 = "1.3.3.8";

            var request = new TriggerDeployment()
            {
                AppPoolName = "ZZZ_Integration_PoolName",
                AppPoolUser = "JackBauer",
                AppPoolPassword = "topsecret",
                WebsiteName = "ZZZ_Integration_Website_VDIR",
                AppRoot = "/sub1/sub2",
                PackageId = "IntegrationTest",
                PackageVersion = version1,
                WebsitePhysicalPath = @"C:\temp\www",
                WebsitePort = 8000,
                RuntimeVersion = RuntimeVersion.Version40,
                AppName = "App",
                AppPhysicalPath = @"c:\temp\int-app",
            };

            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();
            request.PackageVersion = version2;
            client.PostFileWithRequest<TriggerDeploymentResponse>("/deployments", new FileInfo("src/PowerDeploy.IISDeployService.Tests/Files/package.zip".MapVcsRoot()), request).PrintDump();

            var rollbackRequest = new RollbackDeployment()
                {
                    WebsiteName = request.WebsiteName, 
                    RollbackTarget = "{0}_v{1}".Fmt(request.PackageId, version1),
                    AppName = request.AppName,
                    AppRoot = request.AppRoot,
                };

            client.Post(rollbackRequest);

            var actual = new IISManager().GetApplicationPath(request.WebsiteName, request.AppRoot, request.AppName);

            Assert.AreEqual(@"{0}\{1}_v{2}".Fmt(request.AppPhysicalPath, request.PackageId, version1), actual);
        }

        [Test]
        [Category("Integration")]
        public void Try_to_access_service_without_API_Key_Test()
        {
            var client = GetClient();
            client.Headers.Set("X-API-Key", "invalid-API-key");

            try
            {
                client.Send(new RollbackDeployment());
            }
            catch (WebServiceException ex)
            {
                Assert.AreEqual((int)HttpStatusCode.Unauthorized, ex.StatusCode);
            }
        }

        private ServiceClientBase GetClient()
        {
            var client = new JsonServiceClient(Config.ListenOn);
            client.Headers.Add("X-API-Key", "1337");

            return client;
        }
    }
}
