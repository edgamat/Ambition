using System.Reflection;

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Ambition.Api.Monitoring
{
    public class ApplicationInsightsHealthCheckPublisher : IHealthCheckPublisher
    {
        private static TelemetryClient? _client;
        private static readonly object _syncRoot = new();
        private readonly string _connectionString;
        private readonly Dictionary<string, string?> _resourceAttributes;

        public ApplicationInsightsHealthCheckPublisher(string connectionString, Dictionary<string, string?> resourceAttributes)
        {
            _connectionString = connectionString;
            _resourceAttributes = resourceAttributes;
        }

        public Task PublishAsync(HealthReport report, CancellationToken cancellationToken)
        {
            var client = GetOrCreateTelemetryClient();

            SendEventTelemetry(report, client);

            return Task.CompletedTask;
        }

        private void SendEventTelemetry(HealthReport report, TelemetryClient client)
        {
            var properties = _resourceAttributes ?? new Dictionary<string, string?>
                {
                    { "service.instance.id", Environment.MachineName },
                    { "service.name", Assembly.GetEntryAssembly()?.GetName().Name }
                };

            client.TrackEvent("HealthCheck",
                properties: properties,
                metrics: new Dictionary<string, double>
                {
                    { "service.health_check.status", (double)report.Status },
                    { "service.health_check.duration_ms", report.TotalDuration.TotalMilliseconds }
                });
        }

        private TelemetryClient GetOrCreateTelemetryClient()
        {
            if (_client == null)
            {
                lock (_syncRoot)
                {
                    if (_client == null)
                    {
                        var configuration = new TelemetryConfiguration { ConnectionString = _connectionString };
                        _client = new TelemetryClient(configuration);
                    }
                }
            }
            return _client;
        }
    }
}
