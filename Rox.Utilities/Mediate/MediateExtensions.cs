using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Threading;

namespace Rox.Extensions.Mediate
{
    public static class MediateExtensions
    {
        private static readonly Dictionary<Type, Type> _requestTypeToCommandType = new Dictionary<Type, Type>();
        public static IServiceCollection AddMediator(this IServiceCollection services, MediateOptions options, params Assembly[] assemblies)
        {
            var ss = services;
            foreach (var t in assemblies.SelectMany(x => x.DefinedTypes))
            {
                var interfaceType = t.GetInterface(typeof(ICommand<>).FullName);
                if (interfaceType != null)
                {
                    var requestType = interfaceType.GenericTypeArguments[0];
                    if (options.OnlyOneHandler && _requestTypeToCommandType.TryGetValue(requestType, out var oldCommandType))
                    {
                        throw new InvalidCastException($"duplicate request type [{requestType.FullName}] in command type: [{t.FullName}] and [{oldCommandType.FullName}]");
                    }
                    _requestTypeToCommandType[requestType] = t;
                    ss.AddTransient(interfaceType, t);
                    var wrapperType = typeof(CommandWrapperImpl<>).MakeGenericType(requestType);
                    Mediator.RequestTypeToWrapperType[requestType] = wrapperType;
                    ss.AddTransient(wrapperType);
                }
            }
            ss.AddTransient<IMediator, Mediator>();
            return services;
        }
    }
}