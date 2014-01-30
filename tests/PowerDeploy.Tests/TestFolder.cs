using System;
using System.IO;

namespace PowerDeploy.Tests
{
    public class TestFolder : IDisposable
    {
        public DirectoryInfo DirectoryInfo { get; private set; }

        public TestFolder(Environment.SpecialFolder folder)
        {
            DirectoryInfo = Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(folder), Guid.NewGuid().ToString()));
        }

        public void AddFolder(string name)
        {
            Directory.CreateDirectory(Path.Combine(DirectoryInfo.FullName, name));
        }

        public void AddFile(string filename, string content)
        {
            var fileInfo = new FileInfo(Path.Combine(DirectoryInfo.FullName, filename));
            var targetDir = fileInfo.Directory;

            if (!targetDir.Exists)
            {
                targetDir.Create();
            }

            using (var filestream = File.CreateText(Path.Combine(DirectoryInfo.FullName, filename)))
            {
                filestream.Write(content);
                filestream.Flush();
            }
        }

        public void Dispose()
        {
            DirectoryInfo.Delete(true);
        }
    }
}