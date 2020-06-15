using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rox.Modules.Hello
{
    public class HelloModule : ModuleBase
    {
        public override Task ConfigureServices(ServicesConfigureContext context, CancellationToken cancellationToken)
        {
            Console.WriteLine("配置模块: HelloModule");
            return base.ConfigureServices(context, cancellationToken);
        }
    }
}
