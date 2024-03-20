using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ambition.Infrastructure.Data;

internal class AmbitionDbContextDesignTimeDbContextFactory : IDesignTimeDbContextFactory<AmbitionDbContext>
{
    public AmbitionDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly()?.Location) ?? ".")
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<AmbitionDbContextDesignTimeDbContextFactory>(true)
            .Build();

        return new AmbitionDbContext(CreateOptions(configuration, null));
    }

    public static DbContextOptions<AmbitionDbContext> CreateOptions(IConfiguration configuration, IHostEnvironment? environment)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));

        var contextOptions = new DbContextOptionsBuilder<AmbitionDbContext>();

        contextOptions.UseSqlServer(configuration["Database:AmbitionSales"]);

        if (environment?.IsDevelopment() == true)
        {
            contextOptions.LogTo(Console.WriteLine, new[] { RelationalEventId.CommandExecuted }, LogLevel.Warning)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        return contextOptions.Options;
    }
}

