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
            return base.ConfigureServices(context, cancellationToken);
        }
    }
}
