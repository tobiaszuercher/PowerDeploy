using System;
using System.IO;

using PowerDeploy.Core;

namespace PowerDeploy.Server
{
    public class GitWrapper
    {
        private readonly string _directory;
        private string _gitExecutable;
        private Action<string> _output;

        public GitWrapper(string directory, string gitExecutable = "git.exe")
        {
            _directory = directory;
            _output = Console.WriteLine;
            _gitExecutable = gitExecutable;
        }

        public void Clone(string repository)
        {
            new PhysicalFileSystem().EnsureDirectoryExists(_directory);

            SilentProcessRunner.ExecuteCommand(_gitExecutable, "clone -q " + repository + " .", _directory, _output, e => _output("ERROR: " + e));
        }

        public void Pull()
        {
            SilentProcessRunner.ExecuteCommand(_gitExecutable, "pull -q", _directory, _output, e => _output("ERROR: " + e));
        }

        public void PullOrCloneRepository(string repository)
        {
            if (!Directory.Exists(Path.Combine(_directory, ".git")))
            {
                Clone(repository);
            }
        }
    }
}