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

        public string AddFile(string relativeFilename, string content)
        {
            string absoluteFileName = Path.Combine(DirectoryInfo.FullName, relativeFilename);

            var fileInfo = new FileInfo(absoluteFileName);
            var targetDir = fileInfo.Directory;

            if (!targetDir.Exists)
            {
                targetDir.Create();
            }

            using (var filestream = File.CreateText(absoluteFileName))
            {
                filestream.Write(content);
                filestream.Flush();
            }

            return absoluteFileName;
        }

        public string ReadFile(string filename)
        {
            return File.ReadAllText(Path.Combine(DirectoryInfo.FullName, filename));
        }

        public void Dispose()
        {
            DirectoryInfo.Delete(true);
        }
    }
}