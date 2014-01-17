using System;

namespace PowerDeploy.Dashboard.DataAccess.Entities
{
    public class Deployment
    {
        public int Id { get; set; }
        public DateTime ExecutedOn { get; set; }
        public int PackageId { get; set; }
        
        public virtual Package Package { get; set; }
        public int EnvironmentId { get; set; }
    }
}