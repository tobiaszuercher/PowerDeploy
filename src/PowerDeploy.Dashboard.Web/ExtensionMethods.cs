namespace PowerDeploy.Dashboard.Web
{
    public static class ExtensionMethods
    {
        public static string ToHumanReadableBytes(this long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            
            int order = 0;
            while (bytes >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                bytes = bytes / 1024;
            }

            return string.Format("{0:0.##} {1}", bytes, sizes[order]);
        }
    }
}