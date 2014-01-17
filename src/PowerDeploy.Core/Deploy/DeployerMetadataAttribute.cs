using System;
using System.ComponentModel.Composition;

namespace PowerDeploy.Core.Deploy
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DeployerMetadataAttribute : ExportAttribute
    {
        public string PackageType { get; set; }

        public DeployerMetadataAttribute()
            : base(typeof(IDeployer))
        {
        }
    }
}