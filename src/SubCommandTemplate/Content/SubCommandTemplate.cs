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
    /// A value of 0 indicatess success. A value greater than 0 indicates failure
    /// </returns>
    internal async Task<int> OnExecuteAsync
    (
        IConsole console,
        ILogger<SubCommandTemplate> logger
    )
    {
        logger.LogDebug("Starting {command}", GetType().Name);

        try
        {
            // TODO: Validate the command options here if necessary



            // TODO Your code here

        }
        catch (Exception e)
        {
            logger.LogCritical(e, e.Message);
            console.WriteLine(e);
            return ExitCode.ApplicationError;
        }

        logger.LogDebug("Completed {command}", GetType().Name);

        return ExitCode.Success;
    }
}