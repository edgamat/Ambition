using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ambition.Accounting.Data;

internal class AccountingDbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AccountingDbContext>
{
    public AccountingDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location) ?? ".")
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<AccountingDbContextDesignTimeDbContextFactory>(true)
            .Build();

        return new AccountingDbContext(CreateOptions(configuration, null));
    }

    public static DbContextOptions<AccountingDbContext> CreateOptions(IConfiguration configuration, IHostEnvironment? environment)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var contextOptions = new DbContextOptionsBuilder<AccountingDbContext>();

        contextOptions.UseSqlServer(configuration["Database:AmbitionAccounting"]);

        if (environment?.IsDevelopment() == true)
        {
            contextOptions.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted }, LogLevel.Warning)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        return contextOptions.Options;
    }
}

