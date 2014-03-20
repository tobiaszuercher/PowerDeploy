using System.Collections.Generic;
using System.IO;

namespace PowerDeploy.Core
{
    public interface IFileSystem
    {
        IEnumerable<string> EnumerateDirectoryRecursive(string path, string pattern, SearchOption options);

        string ReadFile(string path);
        void OverwriteFile(string path, string content);

        void DeleteFile(string path);

        void DeleteDirectory(string path);

        string CreateTempWorkingDir(string baseDir = @"c:\temp\pd.reloaded");

        void DeleteTempWorkingDirs();

        void EnsureDirectoryExists(string path);

        bool DirectoryExists(string path);

        DirectoryInfo GetParentDirectory(string path);
    }
}