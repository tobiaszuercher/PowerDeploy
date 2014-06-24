using System.Diagnostics;
using System.IO;
using System.Management.Automation;
using System.Web.Security;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.PackageManagerExtension
{
    [Cmdlet(VerbsLifecycle.Invoke, "CreateEncryptionKey")]
    public class InvokeCreateEncryptionKey : PSCmdlet
    {
        public static ILog Log { get; private set; }

        [Parameter(Mandatory = true)]
        public string PasswordFile { get; set; }

        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new PowerShellCommandLineLogFactory();
            Log = LogManager.GetLogger(typeof(InvokeDirectoryTransform));

            var password = Membership.GeneratePassword(64, 0);

            File.WriteAllText(this.PasswordFile, password);

            var fileInfo = new FileInfo(this.PasswordFile);

            Log.InfoFormat("Creating encryption key in '{0}'.", fileInfo.Directory);

            this.OpenExplorerAndSelectFile(fileInfo);
        }

        private void OpenExplorerAndSelectFile(FileInfo fileInfo)
        {
            Process.Start("explorer.exe", string.Format("/select,\"{0}\"", fileInfo.ToString().Replace("/", "\\")));
        }
    }
}