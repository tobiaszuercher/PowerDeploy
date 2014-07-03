using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Web.Security;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.PackageManagerExtension
{
    /// <summary>
    /// Creates a random password and writes it into a file
    /// </summary>
    [Cmdlet(VerbsCommon.New, "EncryptionKey")]
    public class NewEncriptionKey : PSCmdlet
    {
        public static ILog Log { get; private set; }

        [Parameter(Mandatory = true)]
        public string PasswordFile { get; set; }

        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new PowerShellCommandLineLogFactory();
            Log = LogManager.GetLogger(typeof(InvokeDirectoryTransform));

            var password = Membership.GeneratePassword(64, 0);

            File.WriteAllText(PasswordFile, password);

            var fileInfo = new FileInfo(PasswordFile);

            Log.InfoFormat("Creating encryption key in '{0}'.", fileInfo.Directory);

            OpenExplorerAndSelectFile(fileInfo);
        }

        private void OpenExplorerAndSelectFile(FileInfo fileInfo)
        {
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", fileInfo.ToString().Replace("/", "\\")));
        }
    }
}