using MassTransit;

namespace Ambition.Accounting
{
    public class BusControlService : IHostedService
    {
        private readonly IBusControl _busControl;
        private readonly ILogger<BusControlService> _logger;

        public BusControlService(IBusControl busControl, ILogger<BusControlService> logger)
        {
            _busControl = busControl;
            _logger = logger;
        }

        /// <summary>
        /// Starts the underlying <see cref="IBusControl"/>. Once the bus has
        /// been started, it cannot be started again, even after is has been
        /// stopped.
        /// </summary>
        /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting MassTransit bus control.");
            return _busControl.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Stops the underlying <see cref="IBusControl"/>. This method has no
        /// effect if the bus is not started. 
        /// </summary>
        /// <param name="cancellationToken">Indicates that the shutdown process should no longer be graceful.</param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping MassTransit bus control.");
            return _busControl.StopAsync(cancellationToken);
        }
    }
}
