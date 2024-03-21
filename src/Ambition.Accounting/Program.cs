using Ambition.Accounting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.AddMessaging(builder.Configuration, builder.Environment);

var host = builder.Build();
host.Run();
