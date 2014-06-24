using System.Management.Automation;

namespace PowerDeploy.PackageManagerExtension
{
    /// <summary>
    /// Encrypts variable values, removes the
    /// </summary>
    [Cmdlet(VerbsLifecycle.Invoke, "EncryptConfigs")]
    public class InvokeEncryptConfigs : PSCmdlet
    {
        protected override void ProcessRecord()
        {
        }
    }
}