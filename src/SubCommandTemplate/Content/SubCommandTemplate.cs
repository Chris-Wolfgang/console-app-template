// NOTE: If your application was created from the Wolfgang Console App template this
// command is registered automatically at startup (AutoRegisterCommandsConvention).
// In an app without that convention, add [Subcommand(typeof(<this class>))] to Program.
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace {DefaultNamespace}.Command;

[Command
(
    // TODO You can override the default name for the command by uncommenting the line below
    // Name = "{alternative-name}"
    
    // TODO Add a description for your command
    Description = "{command-description}",

    // TODO Specify response file handling. Default is disabled. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.ResponseFileHandling.html
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated

)]
internal class SubCommandTemplate
{
    /// <summary>
    /// This method is called when the command is executed.
    /// </summary>
    /// <returns>
    /// A value of 0 indicates success. A value greater than 0 indicates failure
    /// </returns>
    internal async Task<int> OnExecuteAsync
    (
        IConsole console,
        ILogger<SubCommandTemplate> logger,
        CancellationToken cancellationToken
    )
    {
        logger.LogDebug("Starting {Command}", GetType().Name);

        try
        {
            // TODO: Validate the command options here if necessary



            // TODO Your code here - pass cancellationToken to your async calls so
            // Ctrl+C / host shutdown stops the command gracefully
            await Task.Yield(); // Simulate doing work - remove once your code awaits something
            cancellationToken.ThrowIfCancellationRequested();

        }
        catch (Exception e)
        {
            logger.LogCritical(e, "Unhandled error: {Message}", e.Message);
            console.WriteLine(e);
            return ExitCode.ApplicationError;
        }

        logger.LogDebug("Completed {Command}", GetType().Name);

        return ExitCode.Success;
    }
}