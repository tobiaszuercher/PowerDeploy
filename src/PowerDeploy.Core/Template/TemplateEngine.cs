using System.IO;

namespace PowerDeploy.Core.Template
{
    public class TemplateEngine
    {
        private readonly IFileSystem _fileSystem;

        public IEnviornmentProvider EnviornmentProvider { get; set; }
        public VariableResolver VariableResolver { get; set; }

        public TemplateEngine(IEnviornmentProvider environmentProvider)
            : this(new PhysicalFileSystem(), environmentProvider)
        {
        }

        public TemplateEngine(IFileSystem fileSystem, IEnviornmentProvider environmentProvider)
        {
            _fileSystem = fileSystem;
            EnviornmentProvider = environmentProvider;
        }

        public void TransformDirectory(string path, string environment)
        {
            var targetEnvironment = EnviornmentProvider.GetVariables(environment);

            VariableResolver = new VariableResolver(targetEnvironment.Variables);

            foreach (var templateFile in _fileSystem.EnumerateDirectoryRecursive(path, "*.template.*", SearchOption.AllDirectories))
            {
                var templateText = _fileSystem.ReadFile(templateFile);
                var transformed = VariableResolver.TransformVariables(templateText);
                _fileSystem.OverwriteFile(templateFile.Replace(".template.", "."), transformed);
                _fileSystem.DeleteFile(templateFile);
            }
        }
    }
}