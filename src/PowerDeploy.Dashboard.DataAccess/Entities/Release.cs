using System.Collections.Generic;

namespace PowerDeploy.Dashboard.DataAccess.Entities
{
    public class Release
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Package> Packages { get; set; }
    }
}