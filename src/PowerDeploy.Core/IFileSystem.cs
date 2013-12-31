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

        string CreateTempWorkingDir();

        void DeleteTempWorkingDirs();
    }
}