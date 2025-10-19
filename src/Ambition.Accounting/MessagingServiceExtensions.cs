using Edgamat.Messaging.Configuration;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MessagingServiceExtensions
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAzureServiceBus()
            .WithConfiguration(configuration)
            .AddBusConsumersHostedService()
            .Build();

        return services;
    }
}
