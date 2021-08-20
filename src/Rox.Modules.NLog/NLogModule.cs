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
        public override void ConfigureServices(ServicesConfigureContext context)
        {
            context.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Trace);
                loggingBuilder.AddNLog(context.Configuration);
            });
            base.ConfigureServices(context);
        }
    }
}
