using Ambition.Accounting.Data;

using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

public static class DataServiceExtensions
{
    public static IServiceCollection AddAccountingDbContext(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        // Add DbContextOptions<AccountingDbContext> to the container
        services.AddSingleton(p => AccountingDbContextDesignTimeDbContextFactory.CreateOptions(configuration, environment));

        // Use DbContextOptions<AccountingDbContext> to construct the context
        services.AddScoped(p => new AccountingDbContext(p.GetRequiredService<DbContextOptions<AccountingDbContext>>()));

        return services;
    }

}
