using System;
using System.Linq;

using NuGet;

using PowerDeploy.Core.Deploy;

using ServiceStack;

namespace PowerDeploy.Core.Extensions
{
    public enum PackageType
    {
        XCopy,
        WebApp,
        Database
    }

    public static class ZipPackageExtensions
    {
        public static string PeekPackageType(this ZipPackage package)
        {
            return "xcopy";
        }

        public static T GetPackageOptions<T>(this ZipPackage package) where T : class
        {
            var descriptor = package.GetFiles().FirstOrDefault(f => f.Path == "powerdeploy.xml");

            if (descriptor == null)
            {
                throw new InvalidOperationException("The package {0} has no powerdeploy.xml. Please create a valid package first!".Fmt(package.Id));
            }

            return descriptor.GetStream().FromXml<T>();
        }
    }
}