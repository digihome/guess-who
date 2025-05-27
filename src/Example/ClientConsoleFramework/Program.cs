using System;
using DigiHome.GuessWho.Library;

namespace ClientConsoleFramework
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var result = AppTypeDetector.Detect();

            Console.WriteLine($"Detected application type: {result.Display}");
            
        }
    }
}
