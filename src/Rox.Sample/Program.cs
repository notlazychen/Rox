using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using Rox.Extensions.Hosting;
using Rox.Extensions.Mediate;
using Microsoft.Extensions.DependencyInjection;

namespace Rox.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var host = Host.CreateDefaultBuilder(args)
                .UseRox<AppModule>()
                .Build();

                await host.StartAsync();

            while (true)
            {
                string input = Console.ReadLine();
                if(input == "q")
                {
                    break;
                }
                var mediator = host.Services.GetService<IMediator>();
                await mediator.Send($"[{DateTime.Now:U}] hello world!", CancellationToken.None);
            }
            Console.WriteLine("byebye");
            await host.StopAsync();
        }
    }

    [Dependency(
        typeof(FooModule),
        typeof(Hello2Module),
        typeof(MediateModule)
        )]
    public class AppModule: ModuleBase
    {
    }

    public class Hello1Module : ModuleBase
    {
        public override void ConfigureServices(ServicesConfigureContext context)
        {
            Console.WriteLine("Hello1");
            base.ConfigureServices(context);
        }
    }


    [Dependency(
        typeof(Hello1Module)
        )]
    public class Hello2Module : ModuleBase
    {
        public override void ConfigureServices(ServicesConfigureContext context)
        {
            Console.WriteLine("Hello2");
            base.ConfigureServices(context);
        }
    }

    [Dependency(
        typeof(Hello1Module)
        )]
    public class FooModule : ModuleBase
    {
        public override void ConfigureServices(ServicesConfigureContext context)
        {
            Console.WriteLine("Foo");
            base.ConfigureServices(context);
        }
    }

    public class StringCommand : ICommand<string>
    {
        private Guid _id = Guid.NewGuid();

        public Task ExecuteAsync(string request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{_id}] {request}");
            return Task.CompletedTask;
        }
    }

    public class StringCommand2 : ICommand<string>
    {
        private Guid _id = Guid.NewGuid();

        public Task ExecuteAsync(string request, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[{_id}] {request}");
            return Task.CompletedTask;
        }
    }
}
