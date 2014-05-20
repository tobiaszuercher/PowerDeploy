using System.Collections.Generic;
using System.IO;
using PowerDeploy.Core.Logging;

using ServiceStack;

namespace PowerDeploy.Core.Template
{
    public class TemplateEngine
    {
        private readonly IFileSystem _fileSystem;
        private static readonly ILog Log = LogManager.GetLogger(typeof(VariableResolver));

        public VariableResolver VariableResolver { get; set; }

        public TemplateEngine()
            : this(new PhysicalFileSystem())
        {
        }

        public TemplateEngine(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public int TransformDirectory(string path, Environment targetEnvironment, bool deleteTemplate = true)
        {
            Log.DebugFormat("Transforming {0} for environment {1} {2} deleting templates", path, targetEnvironment.Name, deleteTemplate ? "with" : "without");

            VariableResolver = new VariableResolver(targetEnvironment.Variables);

            int templateCounter = 0;

            foreach (var templateFile in _fileSystem.EnumerateDirectoryRecursive(path, "*.template.*", SearchOption.AllDirectories))
            {
                ++templateCounter;
                Log.InfoFormat("  Transform template {0}", templateFile);

                var templateText = _fileSystem.ReadFile(templateFile);
                var transformed = VariableResolver.TransformVariables(templateText);

                _fileSystem.OverwriteFile(templateFile.Replace(".template.", ".").Replace(".Template.", "."), transformed);

                if (deleteTemplate)
                {
                    _fileSystem.DeleteFile(templateFile);
                }
            }

            Log.DebugFormat("Transformed {0} template(s) in {1}.", templateCounter, path);

            return templateCounter;
        }
    }

    public class TransformResult
    {
        public int TransformedVariables { get; set; }
        public List<string> MissingVariables { get; set; }
    }
}