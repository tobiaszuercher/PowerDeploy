using System;
using System.Collections.Generic;
using System.Linq;

using PowerDeploy.Core;
using PowerDeploy.Server.Mapping;
using PowerDeploy.Server.Model;
using PowerDeploy.Server.Provider;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Deployment;

using Raven.Client;

using ServiceStack;

using Environment = PowerDeploy.Server.Model.Environment;

namespace PowerDeploy.Server.Services
{
    public class DeployService : Service
    {
        public IFileSystem FileSystem { get; set; }
        public ServerSettings ServerSettings { get; set; }
        public NugetPackageDownloader PackageDownloader { get; set; }
        public IDocumentStore DocumentStore { get; set; }

        public TriggerDeploymentResponse Post(TriggerDeployment request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var environment = session.Query<Environment>().FirstOrDefault(e => e.Name.ToUpperInvariant() == request.Environment.ToUpperInvariant());

                if (environment == null)
                {
                    throw HttpError.NotFound("Environment {0} not found.".Fmt(request.Environment));
                }
                
                var deploymentInfo = new Deployment()
                {
                    EnvironmentId = "environments/" + environment.Id,
                    RequestedAt = DateTime.UtcNow,
                };

                var packageInfo = session.Load<Package>("packages/{0}/{1}".Fmt(request.PackageId, request.Version));

                deploymentInfo.PackageId = packageInfo.Id;
                deploymentInfo.Status = DeployStatus.Deploying;
                session.Store(deploymentInfo);

                session.SaveChanges();

                using (var workspace = new Workspace(FileSystem, ServerSettings))
                {
                    var neutralPackagePath = PackageDownloader.Downloaad(request.PackageId, request.Version, workspace.TempWorkDir);

                    workspace.UpdateSources();

                    var packageManager = new PackageManager(workspace.EnviornmentPath);
                    var configuredPackage = packageManager.ConfigurePackageByEnvironment(neutralPackagePath, request.Environment, workspace.TempWorkDir);
                    packageManager.DeployPackage(configuredPackage);
                    deploymentInfo.FinishedAt = DateTime.UtcNow;
                    deploymentInfo.Status = DeployStatus.Successful;
                    session.SaveChanges();

                    return new TriggerDeploymentResponse();
                }
            }
        }

        public List<DeploymentDto> Get(QueryDeploymentsDto request)
        {
            using (var session = DocumentStore.OpenSession())
            {
                var deployments = session.Query<Deployment>()
                    .Include(d => d.EnvironmentId)
                    .Include(d => d.PackageId).ToList()
                    .OrderByDescending(d => d.FinishedAt);

                var result = new List<DeploymentDto>();

                foreach (var deployment in deployments)
                {
                    result.Add(deployment.ToDto(
                          session.Load<Environment>(deployment.EnvironmentId), 
                          session.Load<Package>(deployment.PackageId)));
                }

                return result;
            }
        }
    }

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
            _packages.Add(new Package(nugetId, version) { Published = GetNextDate() });

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