using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

using PowerDeploy.Core.Deploy;

namespace PowerDeploy.Core
{
    public class PluginLoader
    {
        private CompositionContainer _container;

        [ImportMany]
        public Lazy<IDeployer, IDeployerCapabilities>[] Deployers { get; set; }

        public PluginLoader()
        {
            var assemblyCatalog = new AssemblyCatalog(typeof(XCopyDeployer).Assembly);

            //var directoryCatalog = new DirectoryCatalog(@"c:\temp");

            var catalog = new AggregateCatalog(assemblyCatalog/*, directoryCatalog*/);

            _container = new CompositionContainer(catalog);
            _container.ComposeParts(this);
        }
    }
}