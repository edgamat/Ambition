using Ambition.Accounting.Messages;

using MassTransit;

namespace Microsoft.Extensions.DependencyInjection;

public static class MessagingServiceExtensions
{
    public static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddScoped<MaintenancePlanCreatedConsumer>();

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<MaintenancePlanCreatedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ReceiveEndpoint("accounting-maintenance-plan-created", e =>
                {
                    e.Bind("Ambition.Domain:MaintenancePlanCreated", cb => { cb.AutoDelete = false; cb.Durable = true; });
                    e.ConfigureConsumer<MaintenancePlanCreatedConsumer>(context);
                });

                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });
        });

        return services;
    }
}
