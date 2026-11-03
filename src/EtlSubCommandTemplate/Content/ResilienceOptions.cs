// NOTE: Generated alongside EtlSubCommandTemplate because it was created with --resilience.
// Settings for that command's Polly pipeline, bound from the "Resilience" configuration section:
// TODO Add the binding to the ConfigureServices call in Program.cs:
//   .BindConfigSection<ResilienceOptions>("Resilience")
// TODO Add the section to AppSettings.json (any value you leave out keeps the default
// shown here; the section itself must exist with at least one value):
//   "Resilience": {
//       "MaxRetryAttempts": 3,
//       "RetryDelay": "00:00:02",
//       "AttemptTimeout": "00:30:00",
//       "CircuitBreakerFailureRatio": 0.5,
//       "CircuitBreakerMinimumThroughput": 5,
//       "CircuitBreakerSamplingDuration": "00:00:30",
//       "CircuitBreakerBreakDuration": "00:00:30"
//   }
// TODO Move this file to Model/ (and update the namespace) if you prefer to keep it next to SampleConfiguration.

namespace {DefaultNamespace}.Command;

/// <summary>
/// Settings for the resilience pipeline used by <see cref="EtlSubCommandTemplate"/>. Polly
/// enforces its own ranges when the pipeline is built - for example <see cref="AttemptTimeout"/>
/// between 10 ms and 24 h, <see cref="CircuitBreakerMinimumThroughput"/> at least 2 and
/// <see cref="CircuitBreakerFailureRatio"/> between 0 and 1 - and throws a ValidationException otherwise.
/// </summary>
internal record ResilienceOptions
{
    // The setters are invoked by Microsoft.Extensions.Configuration binding via reflection.
    // ReSharper disable UnusedAutoPropertyAccessor.Global

    /// <summary>Retries after the first failed attempt (0 disables retries).</summary>
    public int MaxRetryAttempts { get; init; } = 3;

    /// <summary>Base delay before the first retry; grows exponentially (with jitter) per attempt.</summary>
    public TimeSpan RetryDelay { get; init; } = TimeSpan.FromSeconds(2);

    /// <summary>Time limit for one attempt (the whole extract -> transform -> load pass).</summary>
    public TimeSpan AttemptTimeout { get; init; } = TimeSpan.FromMinutes(30);

    /// <summary>Failure ratio (0..1) within the sampling window that opens the circuit.</summary>
    public double CircuitBreakerFailureRatio { get; init; } = 0.5;

    /// <summary>Minimum attempts within the sampling window before the ratio is evaluated.</summary>
    public int CircuitBreakerMinimumThroughput { get; init; } = 5;

    /// <summary>Length of the window over which the failure ratio is measured.</summary>
    public TimeSpan CircuitBreakerSamplingDuration { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>How long the circuit stays open before a trial attempt is allowed.</summary>
    public TimeSpan CircuitBreakerBreakDuration { get; init; } = TimeSpan.FromSeconds(30);

    // ReSharper restore UnusedAutoPropertyAccessor.Global
}
