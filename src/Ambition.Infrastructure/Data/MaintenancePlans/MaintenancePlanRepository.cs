using Ambition.Domain;

using Microsoft.EntityFrameworkCore;

namespace Ambition.Infrastructure.Data.MaintenancePlans
{
    internal class MaintenancePlanRepository : IMaintenancePlanRepository
    {
        private readonly AmbitionDbContext _dbContext;

        public MaintenancePlanRepository(AmbitionDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(MaintenancePlan maintenancePlan)
        {
            var product = await _dbContext.Set<Product>().FirstOrDefaultAsync(x => x.Id == maintenancePlan.ProductId);

            maintenancePlan.Cost = product?.Price ?? 0m;

            _dbContext.Add(maintenancePlan);

            await _dbContext.SaveChangesAsync();
        }

        public Task<MaintenancePlan?> GetByIdAsync(Guid id)
        {
            return _dbContext.Set<MaintenancePlan>().FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}
