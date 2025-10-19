using Ambition.Domain;

using Edgamat.Messaging;

namespace Ambition.Accounting.Messages
{
    internal class MaintenancePlanCreatedDeadLetterConsumer : JsonConsumer<MaintenancePlanCreated>
    {
        private readonly ILogger<MaintenancePlanCreatedDeadLetterConsumer> _logger;

        public MaintenancePlanCreatedDeadLetterConsumer(ILogger<MaintenancePlanCreatedDeadLetterConsumer> logger) : base(logger)
        {
            _logger = logger;
        }
        public override Task ConsumeMessageAsync(MaintenancePlanCreated message, CancellationToken token)
        {
            _logger.LogInformation("Dead letter message {Id}", message.Id);

            return Task.CompletedTask;
        }
    }

}
