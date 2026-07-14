#if (otel)
using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ConsoleAppTemplate.Framework;

/// <summary>
/// Wires OpenTelemetry tracing + metrics into the host. A console exporter is
/// configured by default; set <c>OpenTelemetry:OtlpEndpoint</c> in AppSettings to
/// also export via OTLP (Jaeger, Grafana, Azure Monitor, Datadog, …).
/// </summary>
internal static class OpenTelemetryHostBuilderExtensions
{
    /// <summary>
    /// Registers OpenTelemetry with the app's <see cref="Telemetry.ActivitySource"/>
    /// and <see cref="Telemetry.Meter"/>.
    /// </summary>
    public static IHostBuilder ConfigureApplicationTelemetry(this IHostBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        return builder.ConfigureServices((context, services) =>
        {
            var otlpEndpoint = context.Configuration["OpenTelemetry:OtlpEndpoint"];

            services.AddOpenTelemetry()
                .ConfigureResource(resource => resource.AddService(Telemetry.ServiceName))
                .WithTracing(tracing =>
                {
                    tracing.AddSource(Telemetry.ServiceName);
                    tracing.AddConsoleExporter();
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    {
                        tracing.AddOtlpExporter(exporter => exporter.Endpoint = new Uri(otlpEndpoint));
                    }
                })
                .WithMetrics(metrics =>
                {
                    metrics.AddMeter(Telemetry.ServiceName);
                    metrics.AddConsoleExporter();
                    if (!string.IsNullOrWhiteSpace(otlpEndpoint))
                    {
                        metrics.AddOtlpExporter(exporter => exporter.Endpoint = new Uri(otlpEndpoint));
                    }
                });
        });
    }
}
#endif
