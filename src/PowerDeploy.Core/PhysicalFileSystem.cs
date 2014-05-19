using System;
using System.Collections.Generic;
using System.IO;

namespace PowerDeploy.Core
{
    public class PhysicalFileSystem : IFileSystem
    {
        private List<string> TempWorkingDirs { get; set; }

        public PhysicalFileSystem()
        {
            TempWorkingDirs = new List<string>();
        }

        public void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public IEnumerable<string> EnumerateDirectoryRecursive(string path, string pattern, SearchOption options)
        {
            return Directory.GetFiles(path, pattern, options);
        }

        public string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void OverwriteFile(string path, string content)
        {
            if (File.Exists(path) && File.GetAttributes(path).HasFlag(FileAttributes.ReadOnly))
            {
                new FileInfo(path).IsReadOnly = false;
            }

            File.WriteAllText(path, content);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public DirectoryInfo GetParentDirectory(string path)
        {
            return Directory.GetParent(path);
        }

        public string CreateTempWorkingDir(string baseDir = @"c:\temp\pd.reloaded")
        {
            // todo: appdata
            var path = Path.Combine(baseDir, Guid.NewGuid().ToString());
            TempWorkingDirs.Add(path);

            return Directory.CreateDirectory(path).FullName;
        }

        public void DeleteTempWorkingDirs()
        {
            TempWorkingDirs.ForEach(path => Directory.Delete(path, true));
        }
    }
}