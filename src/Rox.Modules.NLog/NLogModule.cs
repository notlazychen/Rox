using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rox.Modules.NLog
{
    public class NLogModule : ModuleBase
    {
        public override Task ConfigureServices(ServicesConfigureContext context, CancellationToken cancellationToken)
        {
            context.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(context.Configuration);
            });
            return base.ConfigureServices(context, cancellationToken);
        }
    }
}
