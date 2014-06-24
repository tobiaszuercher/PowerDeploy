using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using NUnit.Framework;

using PowerDeploy.PackageManagerExtension;

namespace PowerDeploy.Tests.TemplateEngineTests
{
    [TestFixture]
    public class EnvironmentEncrypterTest
    {
        [Test]
        public void EncryptVariablesFromEnvironment()
        {
            var envEncryptor = new EnvironmentEncrypter(@"C:\git\PowerDeploy\src\.powerdeploy", "some-secret-aes-key");
            envEncryptor.EncryptAllEnvironments();
        }
    }
}
