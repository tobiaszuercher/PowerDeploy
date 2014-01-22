using System;
using System.Diagnostics;
using System.Management.Automation;

using PowerDeploy.Core;
using PowerDeploy.Core.Extensions;
using PowerDeploy.Core.Logging;
using PowerDeploy.Core.Template;

namespace PowerDeploy.PackageManagerExtension
{
    [Cmdlet(VerbsLifecycle.Invoke, "DirectoryTransform")]
    public class InvokeDirectoryTransform : Cmdlet
    {
        [Parameter(Mandatory = true)]
        public string Environment { get; set; }

        [Parameter(Mandatory = true)]
        public string Directory { get; set; }

        protected override void ProcessRecord()
        {
            LogManager.LogFactory = new CmdletLogFactory(this);

            var xml = new XmlEnvironmentParser(); // TODO: just pass the project dir or current dir and then according to some rules/config files find the folder
            var environment = xml.GetEnvironmentFromFile(@"C:\git\PowerDeploy\src\.powerdeploy\{0}.xml".Fmt(Environment));

            LogManager.GetLogger(GetType()).Info("Test template engine");
            var templateEngine = new TemplateEngine();
            templateEngine.TransformDirectory(Directory, environment, false);
        }
    }
}