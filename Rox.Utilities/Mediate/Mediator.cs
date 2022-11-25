using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Rox.Extensions.Mediate
{
    internal class Mediator : IMediator
    {
        internal static readonly ConcurrentDictionary<Type, Type> RequestTypeToWrapperType = new ConcurrentDictionary<Type, Type>();
        private static readonly ConcurrentDictionary<Type, CommandWrapperBase> _commands = new ConcurrentDictionary<Type, CommandWrapperBase>();
        private readonly IServiceProvider _serviceProvider;

        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var command = _commands.GetOrAdd(request.GetType(), requestType =>
            {
                var wrapperType = RequestTypeToWrapperType[requestType];
                return (CommandWrapperBase)_serviceProvider.GetRequiredService(wrapperType);
            });
            return command.ExecuteAsync(request, cancellationToken);
        }

        public Mediator(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }
    }
}