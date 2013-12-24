using System.Collections.Generic;

using PowerDeploy.DeploymentService.Contract;

using ServiceStack.ServiceInterface;

namespace PowerDeploy.DeploymentService.Services
{
    public class EnvironmentService : Service
    {
         public List<Environment> Get(QueryEnvironment request)
         {
             return new List<Environment>()
                 {
                     new Environment() { Id = 1, Name = "DEV", Description = "Environment for devs." },
                     new Environment() { Id = 2, Name = "TEST", Description = "Test enviornment." },
                     new Environment() { Id = 3, Name = "UAT", Description = "UAT environment" },
                     new Environment() { Id = 4, Name = "PROD", Description = "Environment for devs." }
                 };
         }
    }
}