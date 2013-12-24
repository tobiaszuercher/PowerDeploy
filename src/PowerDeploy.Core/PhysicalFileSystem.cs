using System;
using System.Collections.Generic;
using System.IO;

namespace PowerDeploy.Core
{
    public class PhysicalFileSystem : IFileSystem
    {
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
            return Directory.CreateDirectory(@"c:\temp\pd.reloaded\" + Guid.NewGuid()).FullName;
        }
    }
}