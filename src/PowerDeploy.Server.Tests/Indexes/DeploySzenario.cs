using System;
using System.Collections.Generic;

using PowerDeploy.Server.Model;
using PowerDeploy.Server.ServiceModel.Deployment;

using Raven.Client;
using Raven.Tests.Helpers;

namespace Powerdeploy.Server.Tests.Indexes
{
    public class DeploySzenario
    {
        private DateTime _startDate;
        private readonly IDocumentStore _store;
        private readonly List<Package> _packages;
        private readonly List<Deployment> _deployments;

        public DeploySzenario(IDocumentStore store)
        {
            _store = store;
            _packages = new List<Package>();
            _deployments = new List<Deployment>();
            _startDate = new DateTime(2014, 01, 12);

            AddEnvironments(store);
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

            RavenTestBase.WaitForIndexing(_store);

            return this;
        }

        private void AddEnvironments(IDocumentStore store)
        {
            var e1 = new PowerDeploy.Server.Model.Environment() { Id = (int)Environment.Dev, Name = "DEV" };
            var e2 = new PowerDeploy.Server.Model.Environment() { Id = (int)Environment.Test, Name = "TEST" };
            var e3 = new PowerDeploy.Server.Model.Environment() { Id = (int)Environment.Prod, Name = "PROD" };

            using (var session = store.OpenSession())
            {
                session.Store(e1);
                session.Store(e2);
                session.Store(e3);

                session.SaveChanges();
            }
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

    public static class DocumentStoreExtension
    {
        public static DeploySzenario CreateSzenario(this IDocumentStore target)
        {
            return new DeploySzenario(target);
        }
    }
}