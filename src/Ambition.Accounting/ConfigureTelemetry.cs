using System.Reflection;

using MassTransit.Logging;

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
                resourceBuilder.AddService(builder.Environment.ApplicationName);
                //resourceBuilder.AddService(builder.Environment.ApplicationName, serviceVersion: GetAssemblyVersion());
                //resourceBuilder.AddAttributes(new Dictionary<string, object>
                //{
                //    ["deployment.environment"] = builder.Environment.EnvironmentName,
                //    ["deployment.machine"] = Environment.MachineName,
                //});
            });

        // Logging
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            logging.AddConsoleExporter();
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

                tracing.AddSource(DiagnosticHeaders.DefaultListenerName);

                tracing.AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = true;
                        options.SetDbStatementForStoredProcedure = true;
                    });

                tracing.AddConsoleExporter();
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
