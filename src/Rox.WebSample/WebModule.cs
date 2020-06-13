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

namespace Rox.WebSample
{
    public class WebModule : ModuleBase
    {
        public override Task OnApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            var app = context.ServiceProvider.GetService<IApplicationBuilder>();
            var env = context.ServiceProvider.GetService<IWebHostEnvironment>();

            return Task.CompletedTask;
        }
    }
}
