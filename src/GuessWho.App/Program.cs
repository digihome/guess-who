using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.CommandLine;
using DigiHome.GuessWho.Library;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace DigiHome.GuessWho.App
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var assembly = new Argument<FileInfo>(
                name: "assembly", 
                description: "Path to .dll or .exe to inspect");
            var showReferences = new Option<bool>(
                aliases: new[] { "--full", "-f" }, 
                description: "Show referenced assemblies");
            var noLogo = new Option<bool>(
                aliases: new[] { "--no-logo", "-n" },
                description: "Suppress displaying the startup logo");

            var rootCommand = new RootCommand()
            {
                assembly,
                showReferences,
                noLogo,
            };
            rootCommand.TreatUnmatchedTokensAsErrors = false;
            rootCommand.Description = "GuessWho – CLI and Library for Detecting .NET Application Types";
            rootCommand.Name = "guesswho";

            // Main handler
            rootCommand.SetHandler(async (assembly, showReferences) =>
            {
                RunDetection(assembly, showReferences);
            }, assembly, showReferences);

            // Print logo
            rootCommand.AddValidator(validator =>
            {
                if(!validator.GetValueForOption(noLogo))
                    PrintLogo();
            });

            // If the file name starts with '-', treat it as an option.
            rootCommand.AddValidator(validator =>
            {
                var file = validator.GetValueForArgument(assembly);
                if (file != null && !file.Exists && file.Name.StartsWith("-"))
                {
                    validator.ErrorMessage = $"Required argument missing for command: '{rootCommand.Name}'";
                    return ;
                }
            });

            return rootCommand.InvokeAsync(args).Result;
        }

        /// <summary>
        /// Run the detection on the specified assembly.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="showReferences"></param>
        static void RunDetection(FileInfo file, bool showReferences)
        {
            Console.WriteLine("🕵️ Guess Who...");
            if (!file.Exists)
            {
                Console.WriteLine($"\u001b[31m❌ File not found: {file.FullName}\u001b[0m");
                return;
            }

            MetadataLoadContext mlc;
            Assembly targetAssembly;
            string targetFramework = "Unknown";

            try
            {
                var runtimeAssemblies = GetRuntimeAssemblies();
                var appAssemblies = Directory.GetFiles(file.DirectoryName!, "*.dll");
                var resolver = new PathAssemblyResolver(runtimeAssemblies.Concat(appAssemblies));

                mlc = new MetadataLoadContext(resolver);
                targetAssembly = mlc.LoadFromAssemblyPath(file.FullName);
                var targetFrameworkAttribute = targetAssembly.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute");
                targetFramework = targetFrameworkAttribute.ConstructorArguments[0].Value?.ToString();

                Console.WriteLine($"📁 Assembly: \u001b[36m{file.FullName}\u001b[0m");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\u001b[31m❌ Assembly loading error: {ex.Message}\u001b[0m");
                return;
            }

            var result = AppTypeDetector.Detect(targetAssembly);
            Console.WriteLine($"🔍 Detected: {ColorizeResult(result.Display)}");
            Console.WriteLine($"⚙️ TFM : {targetFramework}");

            if (showReferences)
            {
                var references = targetAssembly.GetReferencedAssemblies()
                    .Select(r => r.Name)
                    .OrderBy(n => n)
                    .ToList();

                if (references.Count > 0)
                {
                    Console.WriteLine("📦 Referenced Assemblies:");
                    foreach (var r in references)
                        Console.WriteLine($"\u001b[35m   - \u001b[0m{r}");
                }
            }
            mlc?.Dispose();
        }

        /// <summary>
        /// Prints the logo to the console.
        /// </summary>
        static void PrintLogo()
        {
            Console.WriteLine(@" _____                     _    _ _            ");
            Console.WriteLine(@"|  __ \                   | |  | | |           ");
            Console.WriteLine(@"| |  \/_   _  ___  ___ ___| |  | | |__   ___   ");
            Console.WriteLine(@"| | __| | | |/ _ \/ __/ __| |/\| | '_ \ / _ \  ");
            Console.WriteLine(@"| |_\ \ |_| |  __/\__ \__ \  /\  / | | | (_) | ");
            Console.WriteLine(@"\____ /\__,_|\___||___/___/\/  \/|_| |_|\___/  ");
            Console.WriteLine();
        }

        /// <summary>
        /// Colorizes the result string based on the detected technology.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        static string ColorizeResult(string result)
        {
            if (result.Contains("Server")) return $"\u001b[32m{result}\u001b[0m";
            if (result.Contains("Client")) return $"\u001b[33m{result}\u001b[0m";
            //if (result.Contains("SignalR")) return $"\u001b[34m{result}\u001b[0m";
            //if (result.Contains("WCF")) return $"\u001b[35m{result}\u001b[0m";
            //if (result.Contains("SOAP")) return $"\u001b[36m{result}\u001b[0m";

            return result;
        }

        /// <summary>
        /// Gets the runtime assemblies for the current process.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<string> GetRuntimeAssemblies()
        {
            var runtimeDir = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
            return Directory.GetFiles(runtimeDir, "*.dll");
        }
    }
}
