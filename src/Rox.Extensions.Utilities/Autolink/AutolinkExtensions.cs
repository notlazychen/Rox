using Microsoft.Extensions.DependencyInjection;
using Rox.Extensions.Mediate;
using System.Reflection;
using System.Threading;

namespace Rox.Extensions.Autolink;

public static class AutolinkExtensions
{
    public static IServiceCollection AddAutolink(this IServiceCollection services, IChannelFactory channelFactory, AutolinkOptions options)
    {
        var ss = services;
        ss.Configure<AutolinkOptions>(o => { o.Address = options.Address; });
        ss.AddSingleton<IChannelFactory>(channelFactory);
        ss.AddSingleton<AutolinkClient>();
        ss.AddHostedService<AutolinkClient>(sp => sp.GetRequiredService<AutolinkClient>());
        return services;
    }

    public static IServiceCollection AddAutolink<TChannelFactory>(this IServiceCollection services, AutolinkOptions options)
        where TChannelFactory : class, IChannelFactory
    {
        var ss = services;
        ss.Configure<AutolinkOptions>(o => { o.Address = options.Address; });
        ss.AddSingleton<TChannelFactory>();
        ss.AddSingleton<IChannelFactory>(sp => sp.GetRequiredService<TChannelFactory>());
        ss.AddSingleton<AutolinkClient>();
        ss.AddHostedService<AutolinkClient>(sp => sp.GetRequiredService<AutolinkClient>());
        return services;
    }
}