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
        Activity.Current?.SetTag(DiagnosticNames.MaintenancePlanId, maintenancePlan.Id);
        Activity.Current?.SetTag(DiagnosticNames.MaintenancePlanProductId, maintenancePlan.ProductId);
        Activity.Current?.SetTag(DiagnosticNames.MaintenancePlanCustomerId, maintenancePlan.CustomerId);

        if (maintenancePlan.Id == Guid.Empty)
        {
            throw new InvalidOperationException("Maintenance plan ID is empty.");
        }

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
