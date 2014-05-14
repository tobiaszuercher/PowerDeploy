using System.IO;
using PowerDeploy.Core.Logging;

using ServiceStack;

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

        public int TransformDirectory(string path, Environment targetEnvironment, bool deleteTemplate = true)
        {
            _logger.InfoFormat("Transforming {0} for environment {1} {2} deleting templates", path, targetEnvironment.Name, deleteTemplate ? "with" : "without");

            VariableResolver = new VariableResolver(targetEnvironment.Variables);

            int templateCounter = 0;

            foreach (var templateFile in _fileSystem.EnumerateDirectoryRecursive(path, "*.template.*", SearchOption.AllDirectories))
            {
                ++templateCounter;
                _logger.InfoFormat("Transforming template {0}", templateFile);

                var templateText = _fileSystem.ReadFile(templateFile);
                var transformed = VariableResolver.TransformVariables(templateText);

                _fileSystem.OverwriteFile(templateFile.Replace(".template.", ".").Replace(".Template.", "."), transformed);

                if (deleteTemplate)
                {
                    _fileSystem.DeleteFile(templateFile);
                }
            }

            _logger.Print("Transformed {0} template(s) in {1}.".Fmt(templateCounter, path));

            return templateCounter;
        }
    }
}