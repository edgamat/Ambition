using Ambition.Domain;
using Ambition.Infrastructure.Events;

using Edgamat.Messaging.Configuration;

using Microsoft.Extensions.Configuration;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class EventServiceExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEventPublisher, EventPublisher>();

        services.AddAzureServiceBus()
            .WithConfiguration(configuration)
            .AddPublisher()
            .Build();

        return services;
    }
}
