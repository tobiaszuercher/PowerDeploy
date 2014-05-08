using System;
using System.Reflection;
using CommandLine;
//using PowerDeploy.Core.Extensions;

namespace PowerDeploy
{
    public class Program
    {
        /// <summary>
        /// powerdeploy config -> default is current sln dir
        /// powerdeploy config bla_v1.3.3.7.nupkg --target-path=c:\temp
        /// powerdeploy deploy bla_v1.3.3.7_DEV.nupkg
        /// powerdeploy deploy --group=frontend ?? where to define the group?
        /// 
        /// should also be possible to say powerdeploy deploy *.nupkg
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            // search for assemblies embedded in the exe (powerdeploy should be just one exe file, no dlls)
            AppDomain.CurrentDomain.AssemblyResolve += SearchInEmbeddedAssembly;
            Foo();
        }

        private static void Foo()
        {
            var test = new CommandLine.AssemblyLicenseAttribute("gugus");
        }

        private static Assembly SearchInEmbeddedAssembly(object sender, ResolveEventArgs args)
        {
            ////var assembly = Assembly.GetExecutingAssembly();

            ////var resourceName = "PowerDeploy.Resources.{0}.dll".Fmt(new AssemblyName(args.Name).Name);

            ////using (var stream = assembly.GetManifestResourceStream(resourceName))
            ////{
            ////    var assemblyData = new Byte[stream.Length];
            ////    stream.Read(assemblyData, 0, assemblyData.Length);

            ////    return Assembly.Load(assemblyData);
            ////}

            return null;
        }
    }
}