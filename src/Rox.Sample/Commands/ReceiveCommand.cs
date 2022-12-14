using Microsoft.Extensions.Logging;
using Rox.Extensions.Mediate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Rox.Sample.Commands
{

    public class ReceiveCommand : ICommand<SendPackageBody>
    {
        private ILogger<ReceiveCommand> _logger;

        public ReceiveCommand(ILogger<ReceiveCommand> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(SendPackageBody e, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"【Receive】{e.Data}");
            return Task.CompletedTask;
        }
    }
}
