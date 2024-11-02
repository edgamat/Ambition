using Ambition.Accounting;
using Ambition.Accounting.Data;
using Ambition.Accounting.Emails;
using Ambition.Accounting.Events;

using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

var builder = Host.CreateApplicationBuilder(args);

builder.ConfigureSerilog();

builder.Services.AddFeatureManagement();

builder.Services.AddHttpClient<IEmailService, EmailService>();

builder.Services.AddHostedService<Worker>();

builder.Services.AddSingleton(p => AccountingDbContextDesignTimeDbContextFactory.CreateOptions(builder.Configuration, builder.Environment));

builder.Services.AddScoped(p => new AccountingDbContext(p.GetRequiredService<DbContextOptions<AccountingDbContext>>()));

builder.Services.AddMessaging();

builder.Services.AddScoped<IEventHandler<MaintenancePlanCreatedEvent>, MaintenancePlanCreatedHandler>();

var host = builder.Build();
host.Run();
