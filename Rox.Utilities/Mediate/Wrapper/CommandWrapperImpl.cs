using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Rox.Extensions.Mediate
{
    internal class CommandWrapperImpl<TRequest> : CommandWrapperBase
    {
        //private ICommand<TRequest> _command;

        //public CommandWrapperImpl(ICommand<TRequest> command)
        //{
        //    _command = command;
        //}
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
}