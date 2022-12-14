using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Rox.Extensions.Mediate
{
    internal abstract class CommandWrapperBase
    {
        public abstract Task ExecuteAsync(object request, CancellationToken cancellationToken);
    }

    internal abstract class CommandWrapperBase<TContext>
    {
        public abstract Task ExecuteAsync(object request, TContext context, CancellationToken cancellationToken);
    }
}