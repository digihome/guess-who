using System.Reflection;

namespace GuessWho.Library
{
    /// <summary>
    /// Application detection class.
    /// </summary>
    public static class AppTypeDetector
    {
        /// <summary>
        /// Detects the application type from the assembly.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static DetectionResult Detect(Assembly assembly)
        {
            var result = new DetectionResult();
            if (assembly == null) return result;

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).ToArray()!;
            }

            // Detect technologies from types
            foreach (var type in types)
            {
                var technologies = AnalyzeTypeForTechnologies(type);
                foreach (var technology in technologies)
                {
                    if (!string.IsNullOrEmpty(technology) && !result.Technologies.Contains(technology))
                        result.Technologies.Add(technology);
                }
            }

            // Detect technologies from references
            if (!result.Technologies.Any(s => s.Contains("Server")) && !result.Technologies.Any(s => s.Contains("Client")))
            {
                var technologies = AnalyzeAssemblyReferencesForTechnologies(assembly);
                foreach (var technology in technologies)
                {
                    if (!string.IsNullOrEmpty(technology) && !result.Technologies.Contains(technology))
                        result.Technologies.Add(technology);
                }
            }

            // If not detected then Client Console
            if (!result.Technologies.Any(s => s.Contains("Server")) && !result.Technologies.Any(s => s.Contains("Client")))
            {
                string targetFramework = "Unknown";
                var targetFrameworkAttribute = assembly.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute");
                if (targetFrameworkAttribute != null)
                    targetFramework = targetFrameworkAttribute.ConstructorArguments[0].Value?.ToString();
                if (targetFramework.StartsWith(".NETFramework") || targetFramework.StartsWith(".NETCoreApp"))
                    result.Technologies.Add("Client Console");
            }

            return result;
        }

        /// <summary>
        /// Analyzes the assembly references for technologies.
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static List<string> AnalyzeAssemblyReferencesForTechnologies(Assembly assembly)
        {
            List<string> technologies = new List<string>();

            var references = assembly.GetReferencedAssemblies()
                .Select(r => r.Name)
                .OrderBy(n => n)
                .ToList();

            foreach (var referenceName in references)
            {
                var technology = DetectTechnology(null, referenceName);
                if (!string.IsNullOrEmpty(technology) && !technologies.Contains(technology))
                    technologies.Add(technology);
            }

            return technologies;
        }

        /// <summary>
        /// Analyzes the type for technologies.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<string> AnalyzeTypeForTechnologies(Type type)
        {
            List<string> technologies = new List<string>();

            // Detect from type
            var technology = DetectTechnology(type.FullName, type.Namespace);
            if (!string.IsNullOrEmpty(technology) && !technologies.Contains(technology))
                technologies.Add(technology);

            // Detect from base type
            var baseType = GetBaseTypeName(type);
            technology = DetectTechnology(baseType.FullName, baseType.Namespace);
            if (!string.IsNullOrEmpty(technology) && !technologies.Contains(technology))
                technologies.Add(technology);
            
            return technologies;
        }

        /// <summary>
        /// Gets the base type name of the type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static (string? FullName, string? Namespace) GetBaseTypeName(Type type)
        {
            try
            {
                return (type.BaseType?.FullName, type.BaseType?.Namespace);
            }
            catch (Exception)
            {
                return (null, null);
            }
        }

        /// <summary>
        /// Detects the technology based on the type full name and namespace.
        /// </summary>
        /// <param name="typeFullName"></param>
        /// <param name="typeNamespace"></param>
        /// <returns></returns>
        private static string DetectTechnology(string typeFullName, string typeNamespace)
        {
            #region Server

            if (typeFullName == "WebApplication" || typeNamespace?.Contains("Microsoft.AspNetCore") == true ||
                typeFullName == "System.Web.Http.ApiController" || typeNamespace?.Contains("System.Web.Http") == true)
            {
                return "Server WebAPI";
            }

            if (typeFullName?.StartsWith("System.Web.Services") == true || typeNamespace == "System.Web.Services")
            {
                return "Server SOAP";
            }

            if (typeFullName?.StartsWith("System.ServiceModel") == true || typeNamespace == "System.ServiceModel")
            {
                return "Server WCF";
            }

            #endregion

            #region Client

            if (typeFullName?.StartsWith("System.Windows.Forms") == true || typeNamespace == "System.Windows.Forms")
            {
                return "Client WinForms";
            }

            #endregion

            #region Service

            if (typeFullName?.StartsWith("System.ServiceProcess") == true || typeNamespace == "System.ServiceProcess" ||
                typeFullName?.StartsWith("Microsoft.Extensions.Hosting.BackgroundService") == true || typeNamespace == "Microsoft.Extensions.Hosting.BackgroundService" ||
                typeFullName?.StartsWith("Microsoft.Extensions.Hosting") == true || typeNamespace?.StartsWith("Microsoft.Extensions.Hosting") == true)
            {
                return "Service";
            }

            #endregion

            return null;
        }
    }
}
