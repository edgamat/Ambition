using System.Reflection;

using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ambition.Api.Monitoring
{
    public static class HealthCheckExtensions
    {
        public static IHostApplicationBuilder ConfigureHealthChecks(this IHostApplicationBuilder builder)
        {
            var healthCheckBuilder = builder.Services.AddHealthChecks();
            healthCheckBuilder.AddCheck("self", () => HealthCheckResult.Healthy());

            var appInsightsConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");

            if (!string.IsNullOrEmpty(appInsightsConnectionString))
            {
                builder.Services
                   .AddSingleton<IHealthCheckPublisher>(sp =>
                   {
                       var resourceAttributes = new Dictionary<string, string?>
                       {
                           { "service.instance.id", Environment.MachineName },
                           { "service.name", builder.Environment.ApplicationName },
                           { "service.version", GetAssemblyVersion() },
                           { "deployment.environment.name", builder.Environment.EnvironmentName },
                       };

                       return new ApplicationInsightsHealthCheckPublisher(appInsightsConnectionString, resourceAttributes);
                   });

                builder.Services.Configure<HealthCheckPublisherOptions>(options => options.Period = TimeSpan.FromMinutes(1));
            }

            return builder;
        }

        public static string GetAssemblyVersion()
        {
            AssemblyInformationalVersionAttribute? infoVersion = Assembly.GetExecutingAssembly()
                .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
                .FirstOrDefault() as AssemblyInformationalVersionAttribute;

            return infoVersion?.InformationalVersion ?? "0.0.0.0";
        }
    }
}
