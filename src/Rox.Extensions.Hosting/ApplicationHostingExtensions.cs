using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rox
{
    public static class ApplicationHostingExtensions
    {
        public static IHostBuilder ConfigureRox<TModule>(this IHostBuilder builder)
            where TModule: ModuleBase, new()
        {
            builder.ConfigureServices((context, services) =>
            {
                using (var provider = services.BuildServiceProvider())
                {
                    var configuration = provider.GetService<IConfiguration>();
                    var loggerFactory = provider.GetService<ILoggerFactory>();
                    var application = new Application(services, configuration, loggerFactory.CreateLogger<Application>());
                    application.Init<TModule>(CancellationToken.None);
                    services.AddSingleton(application);
                    services.AddHostedService<ApplicationHostedService>();
                }
            });
            return builder;
        }
    }
}
