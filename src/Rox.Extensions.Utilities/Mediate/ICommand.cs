using System.Runtime.CompilerServices;

namespace Rox.Extensions.Mediate
{
    public interface ICommand<TRequest>
    {
        Task ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    }

    //public interface ICommand<T1, T2>
    //{
    //    Task ExecuteAsync(T1 arg1, T2 arg2, CancellationToken cancellationToken);
    //}

    //public interface ICommand<TRequest, TContext>
    //{
    //    TContext Context { get; set; }
    //    Task ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    //}
}