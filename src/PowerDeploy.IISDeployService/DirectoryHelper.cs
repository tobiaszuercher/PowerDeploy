using System.IO;
using System.Linq;
using System.Net;

using ServiceStack.Common.Extensions;
using ServiceStack.Text;

namespace PowerDeploy.IISDeployService
{
    public class DirectoryHelper
    {
        public static void DeleteOldFolders(string path, string folderPrefix, int keep)
        {
            new DirectoryInfo(path).GetDirectories("{0}_v*".Fmt(folderPrefix))
                .OrderByDescending(d => d.CreationTimeUtc)
                .Skip(keep)
                .ForEach(d => d.Delete(true));
        }
    }
}