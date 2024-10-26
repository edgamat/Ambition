using System.Reflection;

using Azure.Monitor.OpenTelemetry.Exporter;

using Microsoft.Data.SqlClient;

using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;

using OpenTelemetry.Resources;

using OpenTelemetry.Trace;

namespace Ambition.Accounting;

public static class ConfigureTelemetry
{
    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        var seqServerUrl = builder.Configuration.GetValue<string>("SEQ_SERVER_URL");
        var appInsightsConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");

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
                    ["deployment.environment.name"] = builder.Environment.EnvironmentName
                });
            });

        // Logging
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            if (builder.Environment.IsDevelopment())
            {
                logging.AddConsoleExporter();
            }

            if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
            {
                logging.AddAzureMonitorLogExporter(o => o.ConnectionString = appInsightsConnectionString);
            }

            if (!string.IsNullOrWhiteSpace(seqServerUrl))
            {
                logging.AddOtlpExporter(configure =>
                {
                    configure.Endpoint = new Uri($"{seqServerUrl}/ingest/otlp/v1/logs");
                    configure.Protocol = OtlpExportProtocol.HttpProtobuf;
                    configure.Headers = "X-Seq-ApiKey=yrC3VeSr8KbSsT3MJeiD";
                });
            }
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

                tracing.AddSource(builder.Environment.ApplicationName);

                tracing.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

                tracing.AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = builder.Environment.IsDevelopment();
                        options.SetDbStatementForStoredProcedure = builder.Environment.IsDevelopment();
                        options.Enrich = (activity, name, cmd) =>
                        {
                            if (cmd is SqlCommand sqlCommand)
                                activity.SetTag("db.parameter-count", sqlCommand.Parameters.Count);
                        };
                    });

                if (builder.Environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();
                }

                if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
                {
                    tracing.AddAzureMonitorTraceExporter(o => o.ConnectionString = appInsightsConnectionString);
                }

                if (!string.IsNullOrWhiteSpace(seqServerUrl))
                {
                    tracing.AddOtlpExporter(exporter =>
                    {
                        exporter.Endpoint = new Uri($"{seqServerUrl}/ingest/otlp/v1/traces");
                        exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
                        exporter.Headers = "X-Seq-ApiKey=yrC3VeSr8KbSsT3MJeiD";
                    });
                }
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
