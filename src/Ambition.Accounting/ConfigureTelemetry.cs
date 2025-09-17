using System.Diagnostics;
using System.Reflection;

using Azure.Monitor.OpenTelemetry.Exporter;

using Microsoft.Data.SqlClient;

using OpenTelemetry;
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
        var seqApiKey = builder.Configuration.GetValue<string>("SEQ_API_KEY");
        var appInsightsConnectionString = builder.Configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING");

        var resourceAttributes = new Dictionary<string, object> {
            { "service.name", builder.Environment.ApplicationName },
            { "service.version", GetAssemblyVersion() },
            { "service.namespace", "Ambition" },
            { "service.instance.id", $"{Environment.MachineName}-{Guid.NewGuid()}" }
        };

        var defaultAttributes = new Dictionary<string, object> {
            { "deployment.environment.name", builder.Environment.EnvironmentName }
        };

        // Global settings
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resourceBuilder =>
            {
                resourceBuilder.AddAttributes(resourceAttributes);
            });

        // Logging
        builder.Logging.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;

            // We want to add some default attributes to all our logs
            logging.AddProcessor(new AddAttributesProcessor(defaultAttributes));

            if (builder.Environment.IsDevelopment())
            {
                logging.AddConsoleExporter();

                // Aspire Dashboard
                logging.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
            }

            if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
            {
                logging.AddAzureMonitorLogExporter(o => o.ConnectionString = appInsightsConnectionString);
            }

            if (!string.IsNullOrWhiteSpace(seqServerUrl))
            {
                logging.AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri($"{seqServerUrl}/ingest/otlp/v1/logs");
                    options.Protocol = OtlpExportProtocol.HttpProtobuf;
                    options.Headers = $"X-Seq-ApiKey={seqApiKey}";
                });
            }
        });

        // Tracing
        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                // We want to view all traces
                tracing.SetSampler<AlwaysOnSampler>();

                // We want to add some default attributes to all our traces
                tracing.AddProcessor(new AddSpanTagsProcessor(defaultAttributes));

                // We want to capture custom traces from our application
                tracing.AddSource(builder.Environment.ApplicationName);

                // We want to capture traces from MassTransit
                tracing.AddSource(MassTransit.Logging.DiagnosticHeaders.DefaultListenerName);

                tracing.AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.SetDbStatementForText = builder.Environment.IsDevelopment();
                        options.Enrich = (activity, name, cmd) =>
                        {
                            if (cmd is SqlCommand sqlCommand)
                                activity.SetTag("db.parameter-count", sqlCommand.Parameters.Count);
                        };
                    });

                if (builder.Environment.IsDevelopment())
                {
                    tracing.AddConsoleExporter();

                    // Aspire Dashboard
                    tracing.AddOtlpExporter(options => options.Endpoint = new Uri("http://localhost:4317"));
                }

                if (!string.IsNullOrWhiteSpace(appInsightsConnectionString))
                {
                    tracing.AddAzureMonitorTraceExporter(o => o.ConnectionString = appInsightsConnectionString);
                }

                if (!string.IsNullOrWhiteSpace(seqServerUrl))
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri($"{seqServerUrl}/ingest/otlp/v1/traces");
                        options.Protocol = OtlpExportProtocol.HttpProtobuf;
                        options.Headers = $"X-Seq-ApiKey={seqApiKey}";
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

public class AddAttributesProcessor : BaseProcessor<LogRecord>
{
    private readonly IDictionary<string, object> _attributes;

    public AddAttributesProcessor(IDictionary<string, object> attributes)
    {
        _attributes = attributes;
    }
    public override void OnEnd(LogRecord data)
    {
        var attrs = data.Attributes?.ToList();
        if (attrs is null)
            return;

        // Remove any existing attributes with the same keys
        for (var i = attrs.Count - 1; i >= 0; i--)
        {
            if (_attributes.ContainsKey(attrs[i].Key))
            {
                attrs.RemoveAt(i);
            }
        }

        // Add our attributes
        foreach (var (key, value) in _attributes)
        {
            attrs.Add(new(key, value));
        }

        data.Attributes = attrs;
    }
}

public class AddSpanTagsProcessor : BaseProcessor<Activity>
{
    private readonly IDictionary<string, object> _attributes;

    public AddSpanTagsProcessor(IDictionary<string, object> attributes)
    {
        _attributes = attributes;
    }
    public override void OnEnd(Activity data)
    {
        if (data is null)
            return;

        foreach (var (key, value) in _attributes)
        {
            if (!data.Tags.Any(t => t.Key == key))
            {
                data.SetTag(key, value);
            }
        }
        base.OnEnd(data);
    }
}