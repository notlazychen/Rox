using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Rox
{
    public static class RoxExtensions
    {
        public static IServiceCollection AddApplication<TModule>(this IServiceCollection services) where TModule : ModuleBase, new()
        {
            services.AddSingleton(new TemporaryObject<IApplicationBuilder>());
            using (var provider = services.BuildServiceProvider())
            {
                var configuration = provider.GetService<IConfiguration>();
                var loggerFactory = provider.GetService<ILoggerFactory>();
                var application = new Application(services, configuration, loggerFactory.CreateLogger<Application>());
                application.Init<TModule>(CancellationToken.None);
                services.AddSingleton(application);
            }
            return services;
        }

        public static void UseApplication(this IApplicationBuilder app)
        {
            var tempApp = app.ApplicationServices.GetRequiredService<TemporaryObject<IApplicationBuilder>>();
            tempApp.IsValid = true;
            tempApp.Object = app;
            var application = app.ApplicationServices.GetService<Application>();
            var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            application.Start(CancellationToken.None);
            tempApp.IsValid = false;

            lifetime.ApplicationStopping.Register(() => {
                application.Stop(CancellationToken.None);
            });
        }
    }
}
