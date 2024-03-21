using Ambition.Domain;
using Ambition.Infrastructure.Data;
using Ambition.Infrastructure.Data.MaintenancePlans;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection;

public static class DataServiceExtensions
{
    public static IServiceCollection AddAmbitionDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Add DbContextOptions<AriesContext> to the container
        services.AddSingleton(p => AmbitionDbContextDesignTimeDbContextFactory.CreateOptions(configuration, environment));

        // Use DbContextOptions<AmbitionDbContext> to construct the context
        services.AddScoped(p => new AmbitionDbContext(p.GetRequiredService<DbContextOptions<AmbitionDbContext>>()));

        services.AddScoped<IMaintenancePlanRepository, MaintenancePlanRepository>();

        return services;
    }

}
