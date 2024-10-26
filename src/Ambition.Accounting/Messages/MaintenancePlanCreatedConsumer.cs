using System.Diagnostics;

using Ambition.Accounting.Events;
using Ambition.Domain;

using MassTransit;

namespace Ambition.Accounting.Messages;

public class MaintenancePlanCreatedConsumer : IConsumer<MaintenancePlanCreated>
{
    private readonly ILogger<MaintenancePlanCreatedConsumer> _logger;
    private readonly IEventHandler<MaintenancePlanCreatedEvent> _eventHandler;

    public MaintenancePlanCreatedConsumer(
        ILogger<MaintenancePlanCreatedConsumer> logger,
        IEventHandler<MaintenancePlanCreatedEvent> eventHandler)
    {
        _logger = logger;
        _eventHandler = eventHandler;
    }

    public async Task Consume(ConsumeContext<MaintenancePlanCreated> context)
    {
        _logger.LogInformation("Received MaintenancePlanCreated message for maintenance plan: {Id}", context.Message.Id);

        Activity.Current?.EnrichWithMaintenancePlan(context.Message);

        await _eventHandler.HandleAsync(new MaintenancePlanCreatedEvent
        {
            Id = context.Message.Id,
            ProductId = context.Message.ProductId,
            CustomerId = context.Message.CustomerId,
            CreatedBy = context.Message.CreatedBy,
            CreatedAt = context.Message.CreatedAt
        }, context.CancellationToken);
    }
}
