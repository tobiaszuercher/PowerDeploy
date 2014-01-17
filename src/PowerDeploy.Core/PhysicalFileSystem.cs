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

        public string CreateTempWorkingDir()
        { 
            // todo: appdata
            var path = @"c:\temp\pd.reloaded\" + Guid.NewGuid();
            TempWorkingDirs.Add(path);

            return Directory.CreateDirectory(path).FullName;
        }

        public void DeleteTempWorkingDirs()
        {
            TempWorkingDirs.ForEach(path => Directory.Delete(path, true));
        }
    }
}