using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using NUnit.Framework;

using NuGet;

namespace PowerDeploy.Tests
{
    public static class PackageAssertExtensions
    {
        public static void AssertFileContent(this ZipPackage package, Dictionary<string, string> fileEntries)
        {
            var missmatches = new List<string>();

            foreach (var fileEntry in fileEntries)
            {
                var file = package.GetFiles().FirstOrDefault(f => f.Path == fileEntry.Key);
                
                var content = new StreamReader(file.GetStream(), Encoding.UTF8).ReadToEnd();

                if (fileEntry.Key.EndsWith(".xml") || fileEntry.Key.EndsWith(".config"))
                {
                    content = content.ToXmlOneLine();
                }

                if (content != fileEntry.Value)
                {
                    missmatches.Add(file.Path);
                    
                    Trace.WriteLine("Missmatch in " + fileEntry.Key);
                    Trace.WriteLine("IS:");
                    Trace.WriteLine(content + Environment.NewLine);
                    Trace.WriteLine("SHOULD:");
                    Trace.WriteLine(fileEntry.Value);
                }
                else
                {
                    Trace.WriteLine(fileEntry.Key + " OK");
                }
            }

            if (missmatches.Any())
            {
                Assert.Fail("These files have a missmatch: " + Environment.NewLine + string.Join("," + Environment.NewLine, missmatches));
            }
        }

    }
}