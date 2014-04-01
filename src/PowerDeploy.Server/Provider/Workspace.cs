using System;
using System.IO;

using PowerDeploy.Core;
using PowerDeploy.Server.ServiceModel;

namespace PowerDeploy.Server.Provider
{
    public class Workspace : IDisposable
    {
        public ServerSettings ServerSettings { get; private set; }
        public IFileSystem FileSystem { get; private set; }

        public string TempWorkDir { get; private set; }

        public string RepositoryDir
        {
            get { return Path.Combine(ServerSettings.WorkDir, "repo"); }
        }

        public string EnviornmentPath
        {
            get {  return Path.Combine(RepositoryDir, ServerSettings.EnvironmentsPath); }
        }

        public Workspace(IFileSystem fileSystem, ServerSettings serverSettings)
        {
            ServerSettings = serverSettings;
            FileSystem = fileSystem;

            TempWorkDir = FileSystem.CreateTempWorkingDir(ServerSettings.WorkDir);
        }

        // todo git/tfs decision
        public void UpdateSources()
        {
            var repoDir = Path.Combine(ServerSettings.WorkDir, "repo");
            var git = new GitWrapper(repoDir);
            git.PullOrCloneRepository(ServerSettings.RepositoryUrl);
        }

        public void Dispose()
        {
            FileSystem.DeleteTempWorkingDirs();
        }
    }
}