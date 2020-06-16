using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rox.AspNetCore
{
    public static class ApplicationInitializationContextExtensions
    {
        public static IApplicationBuilder GetApplicationBuilder(this ApplicationInitializationContext context)
        {
            var appTemp = context.ServiceProvider.GetRequiredService<TemporaryObject<IApplicationBuilder>>();
            if (appTemp.IsValid)
                return appTemp.Object;
            throw new InvalidOperationException("Can't find IApplicationBuilder at here!");
        }
    }
}
