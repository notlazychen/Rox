using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Rox.WebSample
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<WebModule>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var application = app.ApplicationServices.GetService<Application>();
            var lifetime = app.ApplicationServices.GetService<IHostApplicationLifetime>();
            application.Start(CancellationToken.None);
            lifetime.ApplicationStopping.Register(()=> {
                application.Stop(CancellationToken.None);
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
