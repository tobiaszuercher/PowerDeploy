using System;
using System.Linq;

using PowerDeploy.Server.Model;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Deployment;
using PowerDeploy.Server.ServiceModel.Package;

using Raven.Abstractions.Indexing;
using Raven.Client.Indexes;

using Environment = PowerDeploy.Server.Model.Environment;

namespace PowerDeploy.Server.Indexes
{
    ////public class Deployment_ByEnvironment : AbstractIndexCreationTask<Deployment>
    ////{
    ////    public class ReducedResult
    ////    {
    ////        public string Id { get; set; }
    ////        public DateTime LastRequestedAt { get; set; }
    ////        public DateTime LastFinishedAt { get; set; }
    ////        public string EnvironmentId { get; set; }
    ////        public string EnvironmentName { get; set; }
    ////        public string NugetId { get; set; }
    ////        public string PackageId { get; set; }
    ////        public int Deployments { get; set; }
    ////        public DeployStatus Status { get; set; }
    ////    }

    ////    public Deployment_ByEnvironment()
    ////    {
    ////        Map = deployments => from deployment in deployments
    ////            select new ReducedResult()
    ////            {
    ////                Id = deployment.Id,
    ////                LastFinishedAt = deployment.FinishedAt,
    ////                LastRequestedAt = deployment.RequestedAt,
    ////                EnvironmentId = deployment.EnvironmentId,
    ////                EnvironmentName = LoadDocument<Environment>(deployment.EnvironmentId).Name,
    ////                NugetId = LoadDocument<Package>(deployment.PackageId).NugetId,
    ////                PackageId = deployment.PackageId,
    ////            };

    ////        Sort(d => d.FinishedAt, SortOptions.None);

    ////    }
    ////}
}