namespace Ambition.Domain
{
    public interface IMaintenancePlanRepository
    {
        Task<MaintenancePlan?> GetByIdAsync(Guid id);

        Task AddAsync(MaintenancePlan maintenancePlan);
    }
}
