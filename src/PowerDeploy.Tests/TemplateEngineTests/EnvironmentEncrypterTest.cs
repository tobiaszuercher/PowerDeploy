﻿using System.Security.Cryptography;
using System.Text;

using NUnit.Framework;

using PowerDeploy.Core;
using PowerDeploy.PackageManagerExtension;

namespace PowerDeploy.Tests.TemplateEngineTests
{
    [TestFixture]
    public class EnvironmentEncrypterTest
    {
        [Test]
        public void EncryptVariablesFromEnvironment()
        {
            // TODO: write some good tests :-)
            //var envEncryptor = new EnvironmentEncrypter(@"C:\git\PowerDeploy\src\.powerdeploy", "some-secret-aes-key");
            //envEncryptor.EncryptAllEnvironments();
        }
    }
}
