using System.IO;

using PowerDeploy.Core;
using PowerDeploy.Core.Logging;

namespace PowerDeploy.Server
{
    /// <summary>
    /// Library support was not enough, so i had to write this wrapper for the command line.
    /// </summary>
    public class GitCommandLineWrapper
    {
        private readonly string _directory;
        private readonly string _gitExecutable;
        private readonly ILog _output;

        public GitCommandLineWrapper(string directory, string gitExecutable = "git.exe")
        {
            _directory = directory;
            _output = LogManager.GetLogger(GetType());
            _gitExecutable = gitExecutable;
        }

        public void Clone(string repository)
        {
            new PhysicalFileSystem().EnsureDirectoryExists(_directory);

            ExecuteGitCommand("clone " + repository + " . -q");
        }

        public void Pull()
        {
            ExecuteGitCommand("pull -q");
        }

        public void PullOrCloneRepository(string repository)
        {
            if (!File.Exists(Path.Combine(_directory, ".git")))
            {
                Clone(repository);
            }
            else
            {
                Pull();
            }
        }

        public void Init()
        {
            new PhysicalFileSystem().EnsureDirectoryExists(_directory);

            ExecuteGitCommand("init");
        }

        private void ExecuteGitCommand(string command)
        {
            SilentProcessRunner.ExecuteCommand(_gitExecutable, command, _directory, o => _output.Info(o), e => _output.Error(e));
        }
    }
}