using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WebTicketSystem
{
    internal class Worker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<Worker> _logger;
        private readonly IConfiguration _configuration;

        public Worker(
            IServiceScopeFactory serviceScopeFactory,
            ILogger<Worker> logger,
            IConfiguration configuration
         )
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _configuration = configuration;
          
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Start TicketSystem");
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Stop TicketSystem");
            return base.StopAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            _logger.LogInformation("TicketSystem Worker running at: {time}", DateTimeOffset.Now);

            try
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{ex.Message}, callStack:{ex.StackTrace}");
            }
            finally
            {
                await Task.Delay(1000, stoppingToken);
            }
            //}
        }
    }
}
