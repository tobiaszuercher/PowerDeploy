using System;
using System.Configuration;

namespace SampleApp.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Value for setting is: {0}", ConfigurationManager.AppSettings["Setting"]);

            Console.ReadLine();
        }
    }
}
