using System.Globalization;
using System.IO;

using PowerDeploy.Core.Logging;

namespace PowerDeploy.Core.Template
{
    public class TemplateEngine
    {
        private readonly IFileSystem _fileSystem;
        private readonly ILog _logger;

        public IEnviornmentProvider EnviornmentProvider { get; set; }
        public VariableResolver VariableResolver { get; set; }

        public TemplateEngine(IEnviornmentProvider environmentProvider)
            : this(new PhysicalFileSystem(), environmentProvider)
        {
        }

        public TemplateEngine(IFileSystem fileSystem, IEnviornmentProvider environmentProvider)
        {
            _logger = LogManager.GetLogger(GetType());
            _fileSystem = fileSystem;
            EnviornmentProvider = environmentProvider;
        }

        public void TransformDirectory(string path, string environment)
        {
            _logger.DebugFormat("Transforming {0} for environment {1}", path, environment.ToUpper(CultureInfo.InvariantCulture));
            var targetEnvironment = EnviornmentProvider.GetVariables(environment);
            targetEnvironment.Variables.Add(new Variable() { Name = "env", Value = environment.ToUpper(CultureInfo.InvariantCulture)});
            targetEnvironment.Variables.Add(new Variable() {Name = "subenv", Value = string.Empty }); // no subenv support for now

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