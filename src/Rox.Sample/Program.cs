using System;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;

namespace Rox.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var host = Host.CreateDefaultBuilder(args)
                .UseRox<AppModule>()
                .Build();

            await host.RunAsync();
        }
    }

    [Dependency(
        typeof(FooModule),
        typeof(Hello2Module)
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
}
