﻿using System.IO;
using System.IO.Packaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PowerDeploy.Tests.TemplateEngineTests
{
    [TestClass]
    public class ZipPackagesTests
    {
         [TestMethod]
         public void Extract_Zip_Package_Test()
         {
             var package = ZipPackage.Open(@"c:\temp\nuget\Testpackage.1.0.0.nupkg", FileMode.Open, FileAccess.ReadWrite);

             var tempFolderPath = @"c:\unzipped";

             foreach (PackagePart part in package.GetParts())
             {
                 var target = Path.GetFullPath(Path.Combine(tempFolderPath, part.Uri.OriginalString.TrimStart('/')));
                 var targetDir = target.Remove(target.LastIndexOf('\\'));

                 if (!Directory.Exists(targetDir))
                     Directory.CreateDirectory(targetDir);

                 using (Stream source = part.GetStream(FileMode.Open, FileAccess.Read))
                 {
                     FileStream targetFile = File.OpenWrite(target);
                     source.CopyTo(targetFile);
                     targetFile.Close();
                 }
             } 
         }
    }
}