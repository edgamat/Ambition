using Ambition.Domain;

using MassTransit;

namespace Ambition.Accounting;

public class MaintenancePlanCreatedConsumer : IConsumer<MaintenancePlanCreated>
{
    private readonly ILogger<MaintenancePlanCreatedConsumer> _logger;

    public MaintenancePlanCreatedConsumer(ILogger<MaintenancePlanCreatedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<MaintenancePlanCreated> context)
    {
        _logger.LogInformation("Received MaintenancePlanCreated event: {Id}", context.Message.Id);

        return Task.CompletedTask;
    }
}
