using System.Reflection;

using Microsoft.ApplicationInsights.Extensibility;

using Serilog;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.Hosting;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ConfigureLogging
{
    public static IHostApplicationBuilder ConfigureSerilog(this IHostApplicationBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.WithProperty("deployment.environment.name", builder.Environment.EnvironmentName)
            .Enrich.WithProperty("service.name", builder.Environment.ApplicationName)
            .Enrich.WithProperty("service.instance.id", Environment.MachineName)
            .Enrich.WithProperty("service.version", GetAssemblyVersion());

        if (builder.Environment.IsDevelopment())
        {
            loggerConfiguration.WriteTo.Console();
        }

        var seqServerUrl = builder.Configuration.GetValue<string>("SEQ_SERVER_URL");
        if (!string.IsNullOrWhiteSpace(seqServerUrl))
        {
            loggerConfiguration.WriteTo.Seq(seqServerUrl);
        }

        var appInsightsConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
        {
            loggerConfiguration.WriteTo.ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces);
        }

        Log.Logger = loggerConfiguration.CreateLogger();

        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog();

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
