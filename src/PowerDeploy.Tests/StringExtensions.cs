using System.IO;
using System.Xml.Linq;

namespace PowerDeploy.Tests
{
    public static class StringExtensions
    {
        public static string ToXmlOneLine(this string xml)
        {
            return XElement.Parse(xml).ToString(SaveOptions.DisableFormatting);
        }

        public static void SetReadOnly(this string filepath)
        {
            new FileInfo(filepath).IsReadOnly = true;
        }
    }
}