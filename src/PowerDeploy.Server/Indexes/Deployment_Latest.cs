using System;
using System.Linq;

using PowerDeploy.Server.Model;
using PowerDeploy.Server.ServiceModel.Deployment;

using Raven.Abstractions.Indexing;
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
            public Package Package { get; set; }
            public string NugetId { get; set; }
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
                                     Package = deployment.Package,
                                     Deployments = 1,
                                     Status = deployment.Status,
                                     NugetId = deployment.Package.NugetId
                                 };


            Reduce = deployments => from deployment in deployments
                                    group deployment by new { PackageName = deployment.Package.NugetId, deployment.EnvironmentId }
                                        into g
                                        select
                                            new ReducedResult()
                                            {
                                                Id = g.OrderByDescending(d => d.FinishedAt).First().Id,
                                                FinishedAt = g.OrderByDescending(d => d.FinishedAt).First().FinishedAt,
                                                RequestedAt = g.OrderByDescending(d => d.FinishedAt).First().RequestedAt,
                                                EnvironmentId = g.OrderByDescending(d => d.FinishedAt).First().EnvironmentId,
                                                EnvironmentName = g.OrderByDescending(d => d.FinishedAt).First().EnvironmentName,
                                                Package = g.OrderByDescending(d => d.FinishedAt).First().Package,
                                                Deployments = g.Sum(d => d.Deployments),
                                                Status = g.OrderByDescending(d => d.FinishedAt).First().Status,
                                                NugetId = g.First().NugetId
                                            };

            Indexes.Add(x => x.Package.NugetId, FieldIndexing.Default);
        }
    }
}