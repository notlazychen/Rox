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
        public static IHostBuilder UseRox<TModule>(this IHostBuilder builder)
            where TModule: ModuleBase, new()
        {
            var application = new Application();
            application.Init<TModule>(builder);
            builder.ConfigureServices((context, services) =>
            {
                services.AddSingleton(application);
                services.AddHostedService<ApplicationHostedService>();
                application.Configure(services, context.Configuration, CancellationToken.None);
            });
            return builder;
        }
    }
}
