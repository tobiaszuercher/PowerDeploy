using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace PowerDeploy.Tests
{
    public class TestBuddy
    {
        public static string GetProjectRoot()
        {
            var root = new DirectoryInfo(new Uri(typeof(TestBuddy).Assembly.CodeBase).LocalPath).Parent.FullName;

            // abuse .gitignore file to find out where the root dir is
            while (!Directory.GetFiles(root).Any(f => f.Contains(".gitignore")))
            {
                Console.WriteLine("DEBUG: " + root);
                Trace.WriteLine("DEBUG: " + root);
                Debug.WriteLine("DEBUG: " + root);

                root = Directory.GetParent(root).FullName;
            }

            return root;
        }

        public static string GetProjectRootCombined(string combinePath)
        {
            return Path.Combine(GetProjectRoot(), combinePath);
        }
    }
}
