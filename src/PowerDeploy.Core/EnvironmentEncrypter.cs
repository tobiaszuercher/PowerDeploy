using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using PowerDeploy.Core.Cryptography;
using PowerDeploy.Core.Logging;

namespace PowerDeploy.Core
{
    /// <summary>
    /// Class to encrypt variables in environments. If they have a do-encrypt attribute, the value will be encrypted using the _aesKey.
    /// Before encryption:
    /// <variable name="Database.password" value="some-secret-password" do-encrypt="true" />
    /// 
    /// After encryption:
    /// <variable name="Firstname" value="(value-is-now-encrypted)" encrypted="true" />
    /// </summary>
    public class EnvironmentEncrypter
    {
        private readonly string _aesKey;

        private EnvironmentProvider _envProvider;

        private ILog Log = LogManager.GetLogger(typeof(EnvironmentEncrypter));

        public EnvironmentEncrypter(string startFolder, string aesKey)
        {
            _envProvider = new EnvironmentProvider();
            _aesKey = aesKey;

            _envProvider.Initialize(startFolder);
        }

        public void EncryptAllEnvironments()
        {
            foreach (var environmentConfigFile in this._envProvider.GetEnvironments(false))
            {
                EncryptEnvironmentConfig(environmentConfigFile);
            }
        }

        private const string RegexFormat = @"<variable (?<spaces_name>\s*)(name=""{0}"")(?<spaces_value>\s*)(value=""(?<value>[^""]+)"") do-encrypt=""([^""]*)""";

        private Regex CreateRegexForVariable(string name)
        {
            return new Regex(string.Format(RegexFormat, name));
        }

        private void EncryptEnvironmentConfig(string configFile)
        {
            var environment = _envProvider.GetEnvironmentFromFile(configFile);
            string environmentAsText = File.ReadAllText(configFile);

            var variablesToEncrypt = environment.Variables.Where(p => p.DoEncrypt).ToList();

            foreach (var variable in variablesToEncrypt)
            {
                var regex = CreateRegexForVariable(variable.Name);

                var encryptedValue = AES.Encrypt(variable.Value, _aesKey);
                environmentAsText = regex.Replace(environmentAsText, @"<variable ${spaces_name}name=""" + variable.Name + @"""${spaces_value}value=""" + encryptedValue + @""" encrypted=""true""");

                Log.InfoFormat("encrypting variable '{0}'", variable.Name);
            }

            File.WriteAllText(configFile, environmentAsText);
        }
    }
}