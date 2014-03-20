using System;
using System.Configuration;

namespace PowerDeploy.SampleApp.ConsoleXCopy
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Gugus");
            Console.WriteLine("Environment: " + ConfigurationManager.AppSettings["env"]);
            Console.WriteLine();
            Console.WriteLine("Variable1: " + ConfigurationManager.AppSettings["variable1"]);
            Console.WriteLine("Variable2: " + ConfigurationManager.AppSettings["variable2"]);
            Console.WriteLine("default.variable: " + ConfigurationManager.AppSettings["default.variable"]);
           
            Console.WriteLine("Press enter to close the application.");
            Console.ReadLine();
        }
    }
}