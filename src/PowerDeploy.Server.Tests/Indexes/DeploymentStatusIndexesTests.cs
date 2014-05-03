using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PowerDeploy.Server;
using PowerDeploy.Server.Indexes;
using PowerDeploy.Server.ServiceModel;

using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Database.Server;
using Raven.Tests.Helpers;

using Xunit;

using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Powerdeploy.Server.Tests.Indexes
{
    [TestClass]
    public class DeploymentStatusIndexesTests : RavenTestBase
    {
        private IDocumentStore _store;

        public DeploymentStatusIndexesTests()
        {
            
            
            //_store = new EmbeddableDocumentStore()
            //{
            //    RunInMemory = true,
            //    UseEmbeddedHttpServer = true,
            //    Url = "http://localhost:7000"
            //}.Initialize();

            //IndexCreation.CreateIndexes(typeof(Environment_ByName).Assembly, _store);
        }

        ~DeploymentStatusIndexesTests()
        {
            _store.Dispose();
        }

        [Fact]
        public void Foo()
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(7001);
            GetNewServer(7000);
            _store = NewDocumentStore();

            new DeploymentStatus_LastDeployed().Execute(_store);

            var p1 = new Package("UnitApp1", "1.0.0.0") { Published = new DateTime(2014, 5, 2) };
            var p2 = new Package("UnitApp1", "1.0.0.1") { Published = new DateTime(2014, 5, 3) };
            var p3 = new Package("OtherApp", "0.0.1.0") { Published = new DateTime(2014, 1, 1) };

            var d1 = new Deployment { EnvironmentId = "environments/1", PackageId = p1.Id, FinishedAt = new DateTime(2014, 1, 1), Status = DeployStatus.Successful };
            var d2 = new Deployment { EnvironmentId = "environments/1", PackageId = p2.Id, FinishedAt = new DateTime(2014, 1, 1), Status = DeployStatus.Successful };

            DataInitializer.InitializerWithDefaultValuesIfEmpty(_store);

            using (var session = _store.OpenSession())
            {
                session.Store(p1);
                session.Store(p2);
                session.Store(p3);

                session.Store(d1);
                session.Store(d2);

                session.SaveChanges();

                WaitForIndexing(_store);

                var results = session.Query<DeploymentStatus_LastDeployed.ReducedResult, DeploymentStatus_LastDeployed>();

                Assert.AreEqual(p2.Id, results.FirstOrDefault().PackageId);
            }
        }

        [TestMethod]
        public void Foo2()
        {
            var szenario = new DeploySzenario(_store);
            szenario
                .PublishPackage("ConsoleApp", "1.0.0.0")
                .PublishPackage("ConsoleApp", "1.0.0.1")
                .Deploy(DeploySzenario.Environment.Dev, "ConsoleApp", "1.0.0.1")
                .Play();

            using (var session = _store.OpenSession())
            {
                var results = session.Query<DeploymentStatus_LastDeployed.ReducedResult, DeploymentStatus_LastDeployed>();

                Assert.AreEqual("1.0.0.1", results.FirstOrDefault().PackageVersion);
                Assert.AreEqual("ConsoleApp", results.FirstOrDefault().PackageName);
                Assert.AreEqual(new Package("ConsoleApp", "1.0.0.1").Id, results.FirstOrDefault().PackageId);
            }
        }

        public class DeploySzenario
        {
            private readonly IDocumentStore _store;
            private DateTime _startDate;
            private readonly List<Package> _packages;
            private readonly List<Deployment> _deployments;

            public DeploySzenario(IDocumentStore store)
            {
                _store = store;
                _packages = new List<Package>();
                _deployments = new List<Deployment>();
                _startDate = new DateTime(2014, 01, 12);

                Initialize(store);
            }

            public DeploySzenario PublishPackage(string nugetId, string version)
            {
                _packages.Add(new Package(nugetId, version) { Published = GetNextDate() } );

                return this;
            }

            public DeploySzenario Deploy(Environment environment, string nugetId, string version)
            {
                var timestamp = GetNextDate();

                _deployments.Add(new Deployment
                {
                    EnvironmentId = "environments/" + (int)environment, 
                    PackageId = new Package(nugetId, version).Id, 
                    RequestedAt = timestamp, 
                    FinishedAt = timestamp.AddMinutes(2),
                    Status = DeployStatus.Successful
                });

                return this;
            }

            public DeploySzenario Play()
            {
                using (var session = _store.OpenSession())
                {
                    _packages.ForEach(session.Store);
                    _deployments.ForEach(session.Store);

                    session.SaveChanges();
                }

                _packages.Clear();
                _deployments.Clear();

                return this;
            }

            private void Initialize(IDocumentStore store)
            {
                var e1 = new PowerDeploy.Server.ServiceModel.Environment() { Id = (int)Environment.Dev, Name = "DEV" };
                var e2 = new PowerDeploy.Server.ServiceModel.Environment() { Id = (int)Environment.Test, Name = "TEST" };
                var e3 = new PowerDeploy.Server.ServiceModel.Environment() { Id = (int)Environment.Prod, Name = "PROD" };

                using (var session = store.OpenSession())
                {
                    session.Store(e1);
                    session.Store(e2);
                    session.Store(e3);

                    session.SaveChanges();
                }
            }

            public  void DeleteAll(IDocumentSession session, string name)
            {
                session.Advanced.DocumentStore.DatabaseCommands.DeleteByIndex("Raven/DocumentsByEntityName", new IndexQuery { Query = "Tag:" + name });
            }

            private DateTime GetNextDate()
            {
                _startDate = _startDate.AddDays(1);

                return _startDate;
            }

            public enum Environment
            {
                Dev = 1,
                Test = 2,
                Prod = 3,
            }
        }

        
    }
}