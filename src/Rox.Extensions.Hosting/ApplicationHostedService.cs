using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rox.Extensions.Hosting
{
    internal class ApplicationHostedService : IHostedService
    {
        private readonly Application _application;

        public ApplicationHostedService(Application application)
        {
            _application = application;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return _application.Start(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _application.Stop(cancellationToken);
        }
    }
}
