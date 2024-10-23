using System.Reflection;

using Azure.Monitor.OpenTelemetry.Exporter;

using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Ambition.UI;

public static class ConfigureTelemetry
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        // Global settings
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder =>
            {
                resourceBuilder.AddService(
                    serviceName: builder.Environment.ApplicationName,
                    serviceVersion: GetAssemblyVersion(),
                    serviceInstanceId: Environment.MachineName);
                resourceBuilder.AddAttributes(new Dictionary<string, object>
                {
                    ["deployment.environment"] = builder.Environment.EnvironmentName
                });
            });

        var connectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");

        // Logging
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            logging.AddConsoleExporter();
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                logging.AddAzureMonitorLogExporter(o => o.ConnectionString = connectionString);
            }
            logging.AddOtlpExporter(configure =>
            {
                configure.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/logs");
                configure.Protocol = OtlpExportProtocol.HttpProtobuf;
                configure.Headers = "X-Seq-ApiKey=yrC3VeSr8KbSsT3MJeiD";
            });
        });

        // Tracing
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    // We want to view all traces in development
                    tracing.SetSampler<AlwaysOnSampler>();
                }

                tracing.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

                tracing.AddAspNetCoreInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                    });

                tracing.AddConsoleExporter();
                if (!string.IsNullOrWhiteSpace(connectionString))
                {
                    tracing.AddAzureMonitorTraceExporter(o => o.ConnectionString = connectionString);
                }

                tracing.AddOtlpExporter(exporter =>
                {
                    exporter.Endpoint = new Uri("http://localhost:5341/ingest/otlp/v1/traces");
                    exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
                    exporter.Headers = "X-Seq-ApiKey=yrC3VeSr8KbSsT3MJeiD";
                });
            });

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
