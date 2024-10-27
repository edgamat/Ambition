using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace Ambition.Domain;

public class MaintenancePlanService : IMaintenancePlanService
{
    private readonly IMaintenancePlanRepository _repository;

    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<MaintenancePlanService> _logger;

    public MaintenancePlanService(IMaintenancePlanRepository repository, IEventPublisher eventPublisher, ILogger<MaintenancePlanService> logger)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Guid> CreateAsync(MaintenancePlan maintenancePlan)
    {
        Activity.Current?.EnrichWithMaintenancePlan(maintenancePlan);

        if (maintenancePlan.Id == Guid.Empty)
        {
            throw new InvalidOperationException("Maintenance plan ID is empty.");
        }

        await _repository.AddAsync(maintenancePlan);

        _logger.LogInformation("Maintenance plan {MaintenancePlanId} created for product {ProductId} and customer {CustomerId}"
            , maintenancePlan.Id, maintenancePlan.ProductId, maintenancePlan.CustomerId);

        var maintenancePlanCreated = new MaintenancePlanCreated(
            maintenancePlan.Id,
            maintenancePlan.ProductId,
            maintenancePlan.CustomerId,
            maintenancePlan.CreatedBy,
            maintenancePlan.CreatedAt);

        await _eventPublisher.PublishAsync(maintenancePlanCreated, "maintenance-plan-created");

        return maintenancePlan.Id;
    }
}
