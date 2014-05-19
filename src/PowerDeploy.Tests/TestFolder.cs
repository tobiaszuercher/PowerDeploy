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

        public TestFolder()
        {
            DirectoryInfo = Directory.CreateDirectory(Path.Combine(System.Environment.CurrentDirectory, Guid.NewGuid().ToString()));
        }

        public void AddFolder(string name)
        {
            Directory.CreateDirectory(Path.Combine(DirectoryInfo.FullName, name));
        }

        public string AddFile(string filename, string content)
        {
            string fileName = Path.Combine(DirectoryInfo.FullName, filename);

            var fileInfo = new FileInfo(fileName);
            var targetDir = fileInfo.Directory;

            if (!targetDir.Exists)
            {
                targetDir.Create();
            }

            using (var filestream = File.CreateText(fileName))
            {
                filestream.Write(content);
                filestream.Flush();
            }

            return filename;
        }

        public void Dispose()
        {
            DirectoryInfo.Delete(true);
        }
    }
}