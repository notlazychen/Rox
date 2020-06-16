using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rox
{
    public class ApplicationHostedService : IHostedService
    {
        private readonly Application _application;

        public ApplicationHostedService(Application application)
        {
            _application = application;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _application.Start(cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _application.Stop(cancellationToken);
            return Task.CompletedTask;
        }
    }
}
