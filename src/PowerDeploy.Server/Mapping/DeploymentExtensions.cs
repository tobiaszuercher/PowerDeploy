using PowerDeploy.Server.Indexes;
using PowerDeploy.Server.Model;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Deployment;
using PowerDeploy.Server.ServiceModel.Environment;
using PowerDeploy.Server.ServiceModel.Package;

using ServiceStack;

namespace PowerDeploy.Server.Mapping
{
    public static class DepylomentExtensions
    {
        public static DeploymentDto ToDto(this Deployment from, Environment environment)
        {
            var to = from.ConvertTo<DeploymentDto>();
            to.Environment = environment.ToDto();
            to.Package = from.Package.ToDto();
            
            //var to = from.ConvertTo<MyDto>();
            //to.Items = from.Items.ConvertAll(x => x.ToDto());
            //to.CalculatedProperty = Calculate(from.Seed);
            return to;
        }

        public static DeploymentDto ToDto(this Deployment_Latest.ReducedResult from, Environment environment)
        {
            var to = from.ConvertTo<DeploymentDto>();
            to.Environment = environment.ToDto();

            return to;
        }

        public static EnvironmentDto ToDto(this Environment from)
        {
            var to = from.ConvertTo<EnvironmentDto>();

            return to;
        }

        public static PackageDto ToDto(this Package from)
        {
            var to = from.ConvertTo<PackageDto>();

            return to;
        }
    }
}