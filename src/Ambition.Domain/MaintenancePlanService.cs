using System.Diagnostics;

namespace Ambition.Domain;

public class MaintenancePlanService : IMaintenancePlanService
{
    private readonly IMaintenancePlanRepository _repository;

    private readonly IEventPublisher _eventPublisher;

    public MaintenancePlanService(IMaintenancePlanRepository repository, IEventPublisher eventPublisher)
    {
        _repository = repository;
        _eventPublisher = eventPublisher;
    }

    public async Task<Guid> CreateAsync(MaintenancePlan maintenancePlan)
    {
        Activity.Current?.SetTag("maintenance-plan.id", maintenancePlan.Id);
        Activity.Current?.SetTag("maintenance-plan.product-id", maintenancePlan.ProductId);
        Activity.Current?.SetTag("maintenance-plan.customer-id", maintenancePlan.CustomerId);

        await _repository.AddAsync(maintenancePlan);

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
