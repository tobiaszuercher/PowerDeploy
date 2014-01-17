using System.Globalization;
using System.IO;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.Core.Template
{
    public class TemplateEngine
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILog _logger;

        public VariableResolver VariableResolver { get; set; }

        public TemplateEngine()
            : this(new PhysicalFileSystem())
        {
        }

        public TemplateEngine(IFileSystem fileSystem)
        {
            _logger = LogManager.GetLogger(GetType());
            _fileSystem = fileSystem;
        }

        public void TransformDirectory(string path, Environment targetEnvironment)
        {
            _logger.DebugFormat("Transforming {0} for environment {1}", path, targetEnvironment.Name);

            ////targetEnvironment.Variables.Add(new Variable() {Name = "subenv", Value = string.Empty }); // no subenv support for now

            VariableResolver = new VariableResolver(targetEnvironment.Variables);

            foreach (var templateFile in _fileSystem.EnumerateDirectoryRecursive(path, "*.template.*", SearchOption.AllDirectories))
            {
                _logger.DebugFormat("Transforming {0}", templateFile);
                var templateText = _fileSystem.ReadFile(templateFile);
                var transformed = VariableResolver.TransformVariables(templateText);
                _fileSystem.OverwriteFile(templateFile.Replace(".template.", ".").Replace(".Template.", "."), transformed);
                _fileSystem.DeleteFile(templateFile);
            }
        }
    }
}