namespace PowerDeploy.Core
{
    public class EnvironmentLocator
    {
        private IFileSystem _fileSystem;

        public IEnvironmentParser EnvironmentParser { get; set; }

        public EnvironmentLocator()
            : this(new XmlEnvironmentParser())
        {
            _fileSystem = new PhysicalFileSystem();
        }

        public EnvironmentLocator(IEnvironmentParser parser)
        {
            
        }
    }
}