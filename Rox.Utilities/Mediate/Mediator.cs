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

        public Mediator(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider;
        }


        public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = default)
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

        //public Task Send<T1, T2>(T1 arg1, T2 arg2, CancellationToken cancellationToken)
        //{
        //    if (arg1 == null)
        //    {
        //        throw new ArgumentNullException(nameof(arg1));
        //    }
        //    if (arg2 == null)
        //    {
        //        throw new ArgumentNullException(nameof(arg2));
        //    }

        //    var command = _commands.GetOrAdd(arg2.GetType(), requestType =>
        //    {
        //        var wrapperType = RequestTypeToWrapperType[requestType];
        //        return (CommandWrapperBase)_serviceProvider.GetRequiredService(wrapperType);
        //    });
        //    return command.ExecuteAsync(arg1, arg2, cancellationToken);
        //}
    }

    //internal class Mediator<TContext> : IMediator<TContext>
    //{
    //    internal static readonly ConcurrentDictionary<Type, Type> RequestTypeToWrapperType = new ConcurrentDictionary<Type, Type>();
    //    private static readonly ConcurrentDictionary<Type, CommandWrapperBase> _commands = new ConcurrentDictionary<Type, CommandWrapperBase>();
    //    private readonly IServiceProvider _serviceProvider;

    //    public Mediator(IServiceProvider serviceProvider)
    //    {
    //        this._serviceProvider = serviceProvider;
    //    }


    //    public Task Send<TRequest>(TRequest request, TContext context, CancellationToken cancellationToken = default)
    //    {
    //        if (request == null)
    //        {
    //            throw new ArgumentNullException(nameof(request));
    //        }

    //        var command = _commands.GetOrAdd(request.GetType(), requestType =>
    //        {
    //            var wrapperType = RequestTypeToWrapperType[requestType];
    //            return (CommandWrapperBase)_serviceProvider.GetRequiredService(wrapperType);
    //        });
    //        command.Context = context;
    //        return command.ExecuteAsync(request, cancellationToken);
    //    }
    //}
}