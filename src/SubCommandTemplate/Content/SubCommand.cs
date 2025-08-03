using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace {DefaultNamespace}.Commands;

[Command
(
    |{AlterativeNameString}|

    --{AlterativeNameReplacement}--

    // TODO Add a description for your command
    Description = "<provide command description>",

    // TODO Specify response file handling. Default is disabled. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.ResponseFileHandling.html
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated

)]
internal class SubCommandTemplate
{



    internal Task<int> OnExecuteAsync
    (
        IConsole console,
        IReporter reporter,
        ILogger<SubCommandTemplate> logger
    )
    {
        logger.LogDebug("Starting {command}", GetType().Name);

        try
        {
            // TODO Your code here

            // Display the value of the currentYear macro
            console.WriteLine($"Current Year: {currentYear}");
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