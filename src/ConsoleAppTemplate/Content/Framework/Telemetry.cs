using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ConsoleAppTemplate.Framework;

/// <summary>
/// Central definitions for the app's OpenTelemetry instrumentation — the
/// <see cref="ActivitySource"/> (traces/spans) and <see cref="Meter"/> (metrics)
/// registered with OpenTelemetry in <c>Program.cs</c>.
/// </summary>
/// <remarks>
/// Usage in a command:
/// <example><code>
/// using var activity = Telemetry.ActivitySource.StartActivity("DoWork");
/// activity?.SetTag("item.count", count);
/// Telemetry.ItemsProcessed.Add(count);
/// </code></example>
/// The console exporter prints spans/metrics to stdout by default; set
/// <c>OpenTelemetry:OtlpEndpoint</c> in AppSettings to also export via OTLP
/// (Jaeger, Grafana, Azure Monitor, Datadog, …).
/// </remarks>
internal static class Telemetry
{
    /// <summary>Name of the application's <see cref="ActivitySource"/> / <see cref="Meter"/>.</summary>
    public const string ServiceName = "ConsoleAppTemplate";



    /// <summary>Activity source for creating spans. Registered via <c>AddSource</c> in Program.cs.</summary>
    public static readonly ActivitySource ActivitySource = new(ServiceName);



    /// <summary>Meter for custom metrics. Registered via <c>AddMeter</c> in Program.cs.</summary>
    public static readonly Meter Meter = new(ServiceName);



    /// <summary>Sample counter — increment it from your commands to demonstrate metrics.</summary>
    public static readonly Counter<long> ItemsProcessed =
        Meter.CreateCounter<long>("app.items_processed", unit: "{item}", description: "Number of items processed.");
}
