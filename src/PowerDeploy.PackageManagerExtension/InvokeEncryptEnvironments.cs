using System.IO;
using System.Management.Automation;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.PackageManagerExtension
{
    /// <summary>
    /// Encrypts variable values, removes the
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "EncryptEnvironments")]
    public class InvokeEncryptEnvironments : PSCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PasswordFile { get; set; }

        [Parameter(Mandatory = true)]
        public string Directory { get; set; }

        public static ILog Log { get; private set; }

        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new PowerShellCommandLineLogFactory();
            Log = LogManager.GetLogger(typeof(InvokeDirectoryTransform));

            if (File.Exists(PasswordFile) == false)
            {
                Log.ErrorFormat("Passwordfile '{0}' doesnt exist! Abording...", PasswordFile);
                return;
            }

            var aesKey = File.ReadAllText(PasswordFile);

            if (string.IsNullOrEmpty(aesKey))
            {
                Log.ErrorFormat("Passwordfile '{0}' is empty! Abording...", PasswordFile);
                return;
            }

            var environmentEncrypter = new EnvironmentEncrypter(Directory, aesKey);
            environmentEncrypter.EncryptAllEnvironments();
        }
    }
}