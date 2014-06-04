using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Raven.Client;

namespace PowerDeploy.Tests
{
    public static class TestExtensionMethods
    {
        public static string MapVcsRoot(this string relativePath)
        {
            var root = new DirectoryInfo(new Uri(typeof(TestExtensionMethods).Assembly.CodeBase).LocalPath).Parent.FullName;

            // abuse .gitignore file to find out where the root dir is
            while (!Directory.GetFiles(root).Any(f => f.Contains(".gitignore")))
            {
                Debug.WriteLine("DEBUG: " + root);
                root = Directory.GetParent(root).FullName;
            }

            return Path.Combine(root, relativePath);
        }

        public static void ClearDocuments<T>(this IDocumentSession session)
        {
            var objects = session.Query<T>().ToList();
            while (objects.Any())
            {
                foreach (var obj in objects)
                {
                    session.Delete(obj);
                }

                session.SaveChanges();
                objects = session.Query<T>().ToList();
            }
        }
    }
}