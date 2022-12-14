using log4net.Core;
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

    public class EchoCommand : ICommand<string>
    {
        private ILogger<EchoCommand> _logger;

        public EchoCommand(ILogger<EchoCommand> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(string e, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"【Echo】{e}");
            return Task.CompletedTask;
        }
    }
}
