﻿using System;

using PowerDeploy.Server.ServiceModel.Deployment;

namespace PowerDeploy.Server.Model
{
    public class Deployment
    {
        public string Id { get; set; }
        public DateTime RequestedAt { get; set; }
        public DateTime FinishedAt { get; set; }
        public string EnvironmentId { get; set; }
        public Package Package { get; set; }
        public DeployStatus Status { get; set; }

        public Deployment()
        {
            Package = new Package();
        }
    }
}
