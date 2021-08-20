using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Rox.AspNetCore;
using Rox.Modules.Hello;
using Rox.Modules.NLog;

namespace Rox.WebSample
{
    [Dependency(
        typeof(NLogModule),
        typeof(HelloModule)
        )]
    public class WebModule : ModuleBase
    {
        public override Task OnApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            var app = context.GetApplicationBuilder();
            var env = context.ServiceProvider.GetService<IWebHostEnvironment>();

            app.UseGlobalLoggerMiddleware();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            return Task.CompletedTask;
        }

        public override void ConfigureServices(ServicesConfigureContext context)
        {
            context.Services.AddControllers();
        }
    }
}
