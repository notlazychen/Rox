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
        public override Task PreApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("程序初始化之前");

            return base.PreApplicationInitialization(context, cancellationToken);
        }

        public override Task OnApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("程序初始化");

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

        public override Task ConfigureServices(ServicesConfigureContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("配置模块");

            context.Services.AddControllers();

            return base.ConfigureServices(context, cancellationToken);
        }

        public override Task OnStopping(CancellationToken cancellationToken)
        {
            Console.WriteLine("程序停止前");

            return base.OnStopping(cancellationToken);
        }
    }
}
