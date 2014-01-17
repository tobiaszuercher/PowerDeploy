using System.Data.Entity;
using System.Runtime.InteropServices;

using PowerDeploy.Dashboard.DataAccess.Entities;

namespace PowerDeploy.Dashboard.DataAccess
{
    public class Context : DbContext
    {
        public DbSet<Deployment> Deployments { get; set; }
        public DbSet<Environment> Environments { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Release> Releases { get; set; }

        public Context()
            : base("PowerDeployConnectionString")
        {
            Database.SetInitializer(new DbInitializer());
        }
    }

    public class DbInitializer : CreateDatabaseIfNotExists<Context>
    {
        protected override void Seed(Context context)
        {
            base.Seed(context);

            context.Environments.Add(new Environment() { Name = "Development", Description = "Environment for dev's. " });
            context.Environments.Add(new Environment() { Name = "Test", Description = "Dedicated Test environment" });
            context.Environments.Add(new Environment() { Name = "Prod", Description = "Production." });
        }
    }
}