using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rox.Extensions.Mediate
{
    public interface IMediator
    {
        Task Send<TRequest>(TRequest request, CancellationToken cancellationToken);
        //Task Send<T1,T2>(T1 arg1, T2 arg2, CancellationToken cancellationToken);
    }

    //public interface IMediator<TContext>
    //{
    //    Task Send<TRequest>(TRequest request, TContext context, CancellationToken cancellationToken);
    //}
}
