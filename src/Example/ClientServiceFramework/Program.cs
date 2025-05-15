using System;
using System.ServiceProcess;
using DigiHome.GuessWho.Library;


namespace ClientServiceFramework
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var result = AppTypeDetector.Detect();

            Console.WriteLine($"Detected application type: {result.Display}");

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
