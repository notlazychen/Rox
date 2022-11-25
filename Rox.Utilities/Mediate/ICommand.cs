using System.Runtime.CompilerServices;

namespace Rox.Extensions.Mediate
{
    public interface ICommand<TRequest>
    {
        Task ExecuteAsync(TRequest request, CancellationToken cancellationToken);
    }

    public interface IMediator
    {
        Task Send<TRequest>(TRequest request, CancellationToken cancellationToken);
    }
}