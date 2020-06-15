using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Rox
{
    public abstract class ModuleBase
    {
        public virtual Task ConfigureServices(ServicesConfigureContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task PreApplicationInitialization(ApplicationInitializationContext context, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public virtual Task OnStopping(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
