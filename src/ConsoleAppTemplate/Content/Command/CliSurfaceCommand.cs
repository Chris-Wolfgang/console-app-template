using System.Text.Json;
using System.Text.Json.Serialization;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate.Command;

/// <summary>
/// Emits a deterministic JSON manifest of this application's <em>CLI surface</em> — its
/// commands, subcommands, options (long/short name + value kind) and positional
/// arguments. The manifest is your app's public contract: scripts and pipelines invoke
/// it with specific flags, so a renamed option or removed subcommand is a breaking
/// change. Commit the manifest as a baseline and diff it between releases to catch drift.
/// </summary>
/// <remarks>
/// Auto-registered as the <c>cli-surface</c> subcommand (see
/// <see cref="Framework.AutoRegisterCommandsConvention"/>). Use <c>--output &lt;file&gt;</c>
/// to write the manifest to a file (recommended - keeps it free of log output); with no
/// option it is written to stdout. See <c>cli-contract/</c> for a baseline check script
/// and CI workflow, and <c>cli-contract/README.md</c> for the "keeping your CLI contract
/// stable" note.
/// </remarks>
[Command
(
    Name = "cli-surface",
    Description = "Emit a deterministic JSON manifest of the app's CLI surface (commands, options, arguments).",
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated
)]
internal class CliSurfaceCommand
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };



    [Option
        (
            Description = "Write the manifest to this file instead of stdout (recommended - avoids mixing log output into the JSON)."
        )
    ]
    public string? Output { get; set; }



    internal async Task<int> OnExecuteAsync
    (
        CommandLineApplication application,
        IConsole console,
        ILogger<CliSurfaceCommand> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug("Starting {Command}", GetType().Name);

        var root = application;
        while (root.Parent is not null)
        {
            root = root.Parent;
        }

        var surface = Describe(root);
        var json = JsonSerializer.Serialize(surface, SerializerOptions);

        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(Output))
        {
            console.WriteLine(json);
        }
        else
        {
            await File.WriteAllTextAsync(Output, json, cancellationToken);
            console.WriteLine($"CLI surface written to {Output}");
        }

        logger.LogDebug("Completed {Command}", GetType().Name);

        return ExitCode.Success;
    }



    /// <summary>
    /// Builds the manifest node for a command and, recursively, its subcommands.
    /// Commands and options are ordered by name so the output is stable across runs;
    /// positional arguments keep declaration order because their position is significant.
    /// </summary>
    private static CommandSurface Describe(CommandLineApplication command)
    {
        var options = command.Options
            .Select(option => new OptionSurface
            (
                option.LongName,
                option.ShortName,
                option.OptionType.ToString()
            ))
            .OrderBy(option => option.LongName ?? option.ShortName, StringComparer.Ordinal)
            .ToList();

        var arguments = command.Arguments
            .Select(argument => new ArgumentSurface
            (
                argument.Name ?? string.Empty,
                argument.MultipleValues
            ))
            .ToList();

        var subcommands = command.Commands
            .Select(Describe)
            .OrderBy(sub => sub.Name, StringComparer.Ordinal)
            .ToList();

        return new CommandSurface(command.Name ?? string.Empty, options, arguments, subcommands);
    }



    private sealed record CommandSurface
    (
        string Name,
        IReadOnlyList<OptionSurface> Options,
        IReadOnlyList<ArgumentSurface> Arguments,
        IReadOnlyList<CommandSurface> Commands
    );



    private sealed record OptionSurface(string? LongName, string? ShortName, string ValueKind);



    private sealed record ArgumentSurface(string Name, bool MultipleValues);
}
