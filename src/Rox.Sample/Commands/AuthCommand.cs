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

    public class AuthCommand : ICommand<AuthResponse>
    {
        private ILogger<AuthCommand> _logger;

        public AuthCommand(ILogger<AuthCommand> logger)
        {
            _logger = logger;
        }

        public Task ExecuteAsync(AuthResponse e, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"【Auth】{e.Ok}, {e.Token}");
            return Task.CompletedTask;
        }
    }
}
