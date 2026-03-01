using System.Diagnostics;

using Ambition.Accounting.Events;
using Ambition.Domain;

using Edgamat.Messaging;

namespace Ambition.Accounting.Messages;

public class MaintenancePlanCreatedConsumer : JsonConsumer<MaintenancePlanCreated>
{
    private readonly ILogger<MaintenancePlanCreatedConsumer> _logger;
    private readonly IEventHandler<MaintenancePlanCreatedEvent> _eventHandler;

    public MaintenancePlanCreatedConsumer(
        ILogger<MaintenancePlanCreatedConsumer> logger,
        IEventHandler<MaintenancePlanCreatedEvent> eventHandler) : base(logger)
    {
        _logger = logger;
        _eventHandler = eventHandler;
    }

    public async override Task ConsumeMessageAsync(MaintenancePlanCreated message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received MaintenancePlanCreated message for maintenance plan: {Id}", message.Id);

        Activity.Current?.EnrichWithMaintenancePlan(message);

        try
        {
            await _eventHandler.HandleAsync(new MaintenancePlanCreatedEvent
            {
                Id = message.Id,
                ProductId = message.ProductId,
                CustomerId = message.CustomerId,
                CreatedBy = message.CreatedBy,
                CreatedAt = message.CreatedAt
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            Activity.Current?.SetStatus(ActivityStatusCode.Error);
            _logger.LogError(ex, "Failed to handle MaintenancePlanCreated message for maintenance plan: {PlanId}", message.Id);
        }
    }
}