using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Rox.Extensions.Mediate
{
    public class MediateModule: ModuleBase
    {
        public override void ConfigureServices(ServicesConfigureContext context)
        {
            var options = context.Configuration.GetSection("mediate").Get<MediateOptions>() ?? new MediateOptions();
            context.Services.AddMediator(options, context.Assemblies.ToArray());
            base.ConfigureServices(context);
        }
    }
}