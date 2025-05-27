using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DigiHome.GuessWho.Library
{
    public class App
    {
        public static IServiceProvider Services => App.host?.Services;

        private static IHost host;
        public static void Init(IHost host)
        {
            App.host = host ?? throw new ArgumentNullException(nameof(host));
            var appType = AppTypeDetector.Detect(Assembly.GetCallingAssembly());
        }
    }
}
