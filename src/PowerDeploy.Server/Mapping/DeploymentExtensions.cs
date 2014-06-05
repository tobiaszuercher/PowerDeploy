using System.Linq;
using PowerDeploy.Server.Model;
using PowerDeploy.Server.ServiceModel;
using PowerDeploy.Server.ServiceModel.Deployment;
using PowerDeploy.Server.ServiceModel.Package;

using ServiceStack;
using ServiceStack.Text;

namespace PowerDeploy.Server.Mapping
{
    public static class DepylomentExtensions
    {
        public static DeploymentDto ToDto(this Deployment from, Environment environment, Package package)
        {
            var to = from.ConvertTo<DeploymentDto>();
            //to.Url = Depl
            to.Environment = environment.ToDto();
            to.Package = package.ToDto();
            
            //var to = from.ConvertTo<MyDto>();
            //to.Items = from.Items.ConvertAll(x => x.ToDto());
            //to.CalculatedProperty = Calculate(from.Seed);
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
            to.Versions = from.Versions.Select(v => new PackageVersionDto()
            {
                Version = v.Version, 
                Published = v.Published
            }).ToList();

            return to;
        }
    }
}