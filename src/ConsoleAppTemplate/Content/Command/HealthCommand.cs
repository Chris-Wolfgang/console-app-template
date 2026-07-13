using System.Text.Json;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate.Command;

/// <summary>
/// Validates that the application's runtime dependencies and configuration are in a
/// working state. Runs a set of lightweight checks and reports each result, exiting
/// non-zero if any check fails.
/// </summary>
/// <remarks>
/// Auto-registered as the <c>health</c> subcommand (see
/// <see cref="Framework.AutoRegisterCommandsConvention"/>). Handy for fast production
/// triage ("config issue or code issue?") and as a container health probe, e.g.
/// <c>HEALTHCHECK CMD myapp health</c>. Use <c>--json</c> for machine-readable output.
/// </remarks>
[Command
(
    Description = "Validate runtime dependencies and configuration (config loaded, log directory writable).",
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated
)]
internal class HealthCommand
{
    [Option
        (
            Description = "Emit machine-readable JSON instead of text - useful for container/orchestrator health probes."
        )
    ]
    public bool Json { get; set; }



    internal async Task<int> OnExecuteAsync
    (
        IConsole console,
        IConfiguration configuration,
        ILogger<HealthCommand> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Starting {Command}", GetType().Name);

        try
        {
            var checks = new List<HealthCheckResult>
            {
                CheckConfigurationLoaded(configuration),
                await CheckLogDirectoryWritableAsync(configuration, cancellationToken),

                // TODO Add your own checks here - e.g. database connectivity, downstream
                // API reachability, or required files/directories. Return a HealthCheckResult
                // from each so it appears in both the text and --json output below.
            };

            cancellationToken.ThrowIfCancellationRequested();

            var healthy = checks.TrueForAll(c => c.Passed);

            if (Json)
            {
                WriteJson(console, checks, healthy);
            }
            else
            {
                WriteText(console, checks);
            }

            logger.LogInformation("Completed {Command} - {Status}", GetType().Name, healthy ? "healthy" : "unhealthy");

            return healthy ? ExitCode.Success : ExitCode.ApplicationError;
        }
        catch (OperationCanceledException e)
        {
            logger.LogWarning(e, "{Command} was canceled", GetType().Name);
            return ExitCode.Canceled;
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled error: {Message}", e.Message);
            await console.Error.WriteLineAsync(e.Message);
            return ExitCode.ApplicationError;
        }
    }



    /// <summary>
    /// Confirms configuration was loaded - i.e. at least one configuration section is present.
    /// </summary>
    private static HealthCheckResult CheckConfigurationLoaded(IConfiguration configuration)
    {
        var loaded = configuration.GetChildren().Any();

        return loaded
            ? HealthCheckResult.Pass("configuration", "Configuration loaded")
            : HealthCheckResult.Fail("configuration", "No configuration sections found");
    }



    /// <summary>
    /// Confirms the log directory (from the Serilog file sink, defaulting to <c>logs/</c>)
    /// exists and is writable by creating and deleting a probe file.
    /// </summary>
    private static async Task<HealthCheckResult> CheckLogDirectoryWritableAsync
    (
        IConfiguration configuration,
        CancellationToken cancellationToken
    )
    {
        // The resolve is inside the try because a malformed configured path can make
        // Path.GetDirectoryName throw (ArgumentException / PathTooLongException).
        var logDirectory = "logs";

        try
        {
            logDirectory = ResolveLogDirectory(configuration);

            Directory.CreateDirectory(logDirectory);

            var probe = Path.Combine(logDirectory, $".health-probe-{Guid.NewGuid():N}.tmp");
            await File.WriteAllTextAsync(probe, string.Empty, cancellationToken);
            File.Delete(probe);

            return HealthCheckResult.Pass("log-directory", $"Log directory writable ({logDirectory})");
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            return HealthCheckResult.Fail("log-directory", $"Log directory not writable ({logDirectory}): {e.Message}");
        }
    }



    /// <summary>
    /// Reads the directory of the first Serilog file sink's <c>path</c> from configuration,
    /// falling back to <c>logs</c> when no file sink path is configured.
    /// </summary>
    private static string ResolveLogDirectory(IConfiguration configuration)
    {
        foreach (var sink in configuration.GetSection("Serilog:WriteTo").GetChildren())
        {
            var path = sink["Args:path"];
            if (!string.IsNullOrWhiteSpace(path))
            {
                var directory = Path.GetDirectoryName(path);
                return string.IsNullOrWhiteSpace(directory) ? "." : directory;
            }
        }

        return "logs";
    }



    private static void WriteText(IConsole console, IReadOnlyList<HealthCheckResult> checks)
    {
        foreach (var check in checks)
        {
            console.WriteLine($"{(check.Passed ? "✅" : "❌")} {check.Detail}");
        }
    }



    private static void WriteJson(IConsole console, IReadOnlyList<HealthCheckResult> checks, bool healthy)
    {
        var payload = new
        {
            status = healthy ? "healthy" : "unhealthy",
            checks = checks.Select(c => new
            {
                name = c.Name,
                status = c.Passed ? "pass" : "fail",
                detail = c.Detail
            })
        };

        console.WriteLine(JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true }));
    }



    private sealed record HealthCheckResult(string Name, bool Passed, string Detail)
    {
        public static HealthCheckResult Pass(string name, string detail) => new(name, Passed: true, detail);



        public static HealthCheckResult Fail(string name, string detail) => new(name, Passed: false, detail);
    }
}
