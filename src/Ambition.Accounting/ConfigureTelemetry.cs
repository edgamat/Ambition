using System.Reflection;

using Microsoft.ApplicationInsights.Extensibility;

using Serilog;

namespace Ambition.Accounting;

public static class ConfigureTelemetry
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .Enrich.WithProperty("deployment.environment.name", builder.Environment.EnvironmentName)
            .Enrich.WithProperty("service.name", builder.Environment.ApplicationName)
            .Enrich.WithProperty("service.version", GetAssemblyVersion())
            .WriteTo.Console()
            .WriteTo.Seq("http://localhost:5341");

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
