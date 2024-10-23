using System.Reflection;

using Azure.Monitor.OpenTelemetry.Exporter;

using Microsoft.Data.SqlClient;

using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Ambition.Accounting;

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

        // Logging
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            logging.AddConsoleExporter();
            // logging.AddAzureMonitorLogExporter(o => o.ConnectionString = "InstrumentationKey=21852173-8ded-4414-ab32-3ec2dc7845b5;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=da5fcc28-11dd-434a-b816-7cdcb867c4d7");
            var connectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                logging.AddAzureMonitorLogExporter(o =>
                {
                    o.ConnectionString = connectionString;
                });
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

                tracing.AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                        options.Enrich = (activity, name, cmd) =>
                        {
                            if (cmd is SqlCommand sqlCommand)
                                activity.SetTag("db.parameter-count", sqlCommand.Parameters.Count);
                        };
                    });

                tracing.AddConsoleExporter();
                tracing.AddAzureMonitorTraceExporter(o => o.ConnectionString = "InstrumentationKey=21852173-8ded-4414-ab32-3ec2dc7845b5;IngestionEndpoint=https://eastus-8.in.applicationinsights.azure.com/;LiveEndpoint=https://eastus.livediagnostics.monitor.azure.com/;ApplicationId=da5fcc28-11dd-434a-b816-7cdcb867c4d7");
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
