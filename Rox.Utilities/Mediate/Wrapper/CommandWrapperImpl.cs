using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Rox.Extensions.Mediate
{
    internal class CommandWrapperImpl<TRequest> : CommandWrapperBase
    {
        private IEnumerable<ICommand<TRequest>> _commands;

        public CommandWrapperImpl(IEnumerable<ICommand<TRequest>> commands)
        {
            _commands = commands;
        }

        public override async Task ExecuteAsync(object request, CancellationToken cancellationToken)
        {
            foreach (var command in _commands)
            {
                await command.ExecuteAsync((TRequest)request, cancellationToken);
            }
        }
    }

    //internal class CommandWrapperImpl<TRequest, TContext> : CommandWrapperBase<TContext>
    //{
    //    private IEnumerable<ICommand<TRequest, TContext>> _commands;

    //    public CommandWrapperImpl(IEnumerable<ICommand<TRequest, TContext>> commands)
    //    {
    //        _commands = commands;
    //    }

    //    public override async Task ExecuteAsync(object request, TContext context, CancellationToken cancellationToken)
    //    {
    //        foreach (var command in _commands)
    //        {
    //            command.Context = context;
    //            await command.ExecuteAsync((TRequest)request, cancellationToken);
    //        }
    //    }
    //}
}