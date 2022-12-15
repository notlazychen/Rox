using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Threading;
using Rox.Extensions.Mediate;

namespace Rox.Extensions.Autolink;

public class AutolinkModule : ModuleBase
{
    public override void ConfigureServices(ServicesConfigureContext context)
    {
        var options = context.Configuration.GetSection("mediate").Get<AutolinkOptions>() ?? new AutolinkOptions();
        //context.Services.AddAutolink(options);
        base.ConfigureServices(context);
    }
}