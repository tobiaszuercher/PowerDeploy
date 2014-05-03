using System;
using System.Linq;

using PowerDeploy.Server.Model;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Deployment;

using Raven.Client.Indexes;

using Environment = PowerDeploy.Server.Model.Environment;

namespace PowerDeploy.Server.Indexes
{
    public class Deployment_Latest : AbstractIndexCreationTask<Deployment, Deployment_Latest.ReducedResult>
    {
        public class ReducedResult
        {
            public string Id { get; set; }
            public DateTime RequestedAt { get; set; }
            public DateTime FinishedAt { get; set; }
            public string EnvironmentId { get; set; }
            public string EnvironmentName { get; set; }
            public string PackageId { get; set; }
            public string NugetId { get; set; }
            public string PackageVersion { get; set; }
            public int Deployments { get; set; }
            public DeployStatus Status { get; set; }
        }

        public Deployment_Latest()
        {
            Map = deployments => from deployment in deployments 
                                 select new ReducedResult()
                                 {
                                     Id = deployment.Id, 
                                     FinishedAt = deployment.FinishedAt, 
                                     RequestedAt = deployment.RequestedAt,
                                     EnvironmentId = deployment.EnvironmentId,
                                     EnvironmentName = LoadDocument<Environment>(deployment.EnvironmentId).Name,
                                     NugetId = LoadDocument<Package>(deployment.PackageId).NugetId,
                                     PackageVersion = LoadDocument<Package>(deployment.PackageId).Version,
                                     PackageId = deployment.PackageId,
                                     Deployments = 1,
                                     Status = deployment.Status
                                 };


            Reduce = deployments => from deployment in deployments
                group deployment by new { PackageName = deployment.NugetId, deployment.EnvironmentId }
                into g
                select
                    new ReducedResult()
                    {
                        EnvironmentId = g.OrderByDescending(d => d.FinishedAt).First().EnvironmentId,
                        EnvironmentName = g.OrderByDescending(d => d.FinishedAt).First().EnvironmentName,
                        Deployments = g.Sum(d => d.Deployments),
                        FinishedAt = g.OrderByDescending(d => d.FinishedAt).First().FinishedAt,
                        RequestedAt = g.OrderByDescending(d => d.FinishedAt).First().RequestedAt,
                        PackageId = g.OrderByDescending(d => d.FinishedAt).First().PackageId,
                        PackageVersion = g.OrderByDescending(d => d.FinishedAt).First().PackageVersion,
                        Id = g.OrderByDescending(d => d.FinishedAt).First().Id,
                        NugetId = g.OrderByDescending(d => d.FinishedAt).First().NugetId,
                        Status = g.OrderByDescending(d => d.FinishedAt).First().Status
                    };
        }
    }
}