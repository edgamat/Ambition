using Ambition.Accounting;
using Ambition.Accounting.Data;
using Ambition.Accounting.Events;

using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

// Add DbContextOptions<AriesContext> to the container
builder.Services.AddSingleton(p => AccountingDbContextDesignTimeDbContextFactory.CreateOptions(builder.Configuration, builder.Environment));

// Use DbContextOptions<AmbitionDbContext> to construct the context
builder.Services.AddScoped(p => new AccountingDbContext(p.GetRequiredService<DbContextOptions<AccountingDbContext>>()));


builder.Services.AddMessaging(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IEventHandler<MaintenancePlanCreatedEvent>, MaintenancePlanCreatedHandler>();

var host = builder.Build();
host.Run();
