using System;
using Microsoft.Extensions.Hosting;
using Rox.Modules.NLog;
using Rox.Modules.Hello;
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
        typeof(NLogModule),
        typeof(HelloModule)
        )]
    public class AppModule: ModuleBase
    {
    }
}
