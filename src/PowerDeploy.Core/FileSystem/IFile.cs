namespace PowerDeploy.Core.FileSystem
{
    public interface IFile
    {
        string Name { get; set; }
        string Extension { get; set; }

        string FullName { get; set; }
        bool Exists { get; }

        string GetContent();

        bool WriteContent();
    }
}