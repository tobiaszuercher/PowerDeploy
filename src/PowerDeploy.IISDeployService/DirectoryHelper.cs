using System.IO;
using System.Linq;

using ServiceStack;

namespace PowerDeploy.IISDeployService
{
    public class DirectoryHelper
    {
        public static void DeleteOldFolders(string path, string folderPrefix, int keep)
        {
            foreach (var directory in new DirectoryInfo(path).GetDirectories("{0}_v*".Fmt(folderPrefix)).OrderByDescending(d => d.CreationTimeUtc).Skip(keep))
            {
                directory.Delete(true);
            }
        }
    }
}