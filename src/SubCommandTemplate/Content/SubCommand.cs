using System;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace SubCommandTemplate.Content;

[Command
(
    // TODO You can override the default name for the command by uncommenting the line below
    //Name = "<override with command if needed>",

    // TODO Add a description for your command
    Description = "<add description here>",

    // TODO Specify response file handling. Default is disabled. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.ResponseFileHandling.html
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated

)]
internal class SubCommand
{



    internal Task<int> OnExecuteAsync
    (
        IConsole console,
        IReporter reporter,
        ILogger<SubCommand> logger
    )
    {
        logger.LogDebug("Starting {command}", GetType().Name);

        throw new NotImplementedException("This command is not implemented yet.");

        try
        {
            // TODO Your code here

        }
        catch (Exception e)
        {
            logger.LogCritical(e, e.Message);
            console.WriteLine(e);
            // TODO Uncomment the line below to return an error exit code
            //return ExitCode.ApplicationError;
        }


        logger.LogDebug("Completed {command}", GetType().Name);

        // TODO Uncomment the line below to return a success exit code
        //return ExitCode.Success;
    }
}