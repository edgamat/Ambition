
namespace Ambition.Domain;

public interface IMaintenancePlanService
{
    Task<Guid> CreateAsync(MaintenancePlan maintenancePlan);
}