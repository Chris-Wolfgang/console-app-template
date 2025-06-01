using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace SubCommandTemplate.Command;

[Command
(
    // TODO You can override the default name for the command by uncommenting the line below
    //Name = "<override with command if needed>",

    // TODO Add a description for your command
    Description = "<add description here>"
)]
internal class SubCommand
{


    [Argument(0, Description = "Filename and path to the file to process")]
    [Required]
    [FileExists]
    public string SourcePath { get; set; }



    [Argument(1, Description = "Filename and path to receive the output")]
    [Required]
    public string DestinationPath { get; set; }






    internal async Task<int> OnExecuteAsync
    (
        IConsole console,
        IReporter reporter,
        ILogger<SubCommand> logger
    )
    {
        logger.LogDebug("Starting {command}", GetType().Name);

        try
        {
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