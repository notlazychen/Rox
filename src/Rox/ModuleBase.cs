using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace Rox
{
    public abstract class ModuleBase
    {
        public virtual IHostBuilder ConfigureHost(IHostBuilder builder)
        {
            return builder;
        }

        public virtual void ConfigureServices(ServicesConfigureContext context)
        {
        }

        public virtual Task OnApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task PreApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnStopping(ApplicationShutdownContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
