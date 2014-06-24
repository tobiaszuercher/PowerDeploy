using System.Linq;

using PowerDeploy.Core;
using PowerDeploy.Core.Cryptography;

namespace PowerDeploy.PackageManagerExtension
{
    /// <summary>
    /// Scans all variables in all environments. If they have a do-encrypt attribute, the value will be encrypted using the aesKey.
    /// Before encryption:
    /// <variable name="Database.password" value="some-secret-password" do-encrypt="true" />
    /// 
    /// After encryption:
    /// <variable name="Firstname" value="(value-is-now-encrypted)" encrypted="true" />
    /// </summary>
    public class EnvironmentEncrypter
    {
        private readonly string aesKey;

        private EnvironmentProvider envProvider;

        public EnvironmentEncrypter(string startFolder, string aesKey)
        {
            this.envProvider = new EnvironmentProvider();
            this.aesKey = aesKey;

            envProvider.Initialize(startFolder);
        }

        public void EncryptAllEnvironments()
        {
            foreach (var environmentConfigFile in envProvider.GetEnvironments(false))
            {
                this.EncryptEnvironmentConfig(environmentConfigFile);
            }
        }

        private void EncryptEnvironmentConfig(string configFile)
        {
            var environment = this.envProvider.GetEnvironmentFromFile(configFile);

            var variablesToEncrypt = environment.Variables.Where(p => p.DoEncrypt).ToList();

            foreach (var variable in variablesToEncrypt)
            {
                variable.DoEncrypt = false;
                variable.Encrypted = true;
                variable.Value = AES.Encrypt(variable.Value, this.aesKey);
            }
        }
    }
}