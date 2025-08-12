using System.ComponentModel.DataAnnotations;
using ConsoleAppTemplate.Model;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace ConsoleAppTemplate.Command;

[Command
(
    // TODO You can override the default name for the command by uncommenting the line below
    //Name = "<override with command if needed>",

    // TODO Add a description for your command
    Description = "<add description here>",

    // TODO Specify response file handling. Default is disabled. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.ResponseFileHandling.html
    ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated

)]
internal class SampleCommand
{


    [FileExists]
    [Required]
    [Argument(0, Description = "Filename and path to the file to process")]
    public required string SourcePath { get; set; }



    [Required]
    [Argument(1, Description = "Filename and path to receive the output")]
    public required string DestinationPath { get; set; }




    [Option
        (
            Description = "Character to separate values",
            ValueName = ";"
        )
    ]
    public char Delimiter { get; set; } = ',';


    
    [Option
        (
            Description = "Number of lines to process"
        )
    ]
    // Ensures the user does not enter a negative number
    [Range(0, 10_000)] 
    public int MaxLines { get; set; } = 1000;



    [Option
        (
            Description = "When specified, sends an email notification to the specified address"
        )
    ]
    [EmailAddress]
    [MaxLength(255)]
    public string? EmailAddress { get; set; }


    internal async Task<int> OnExecuteAsync
    (
        SampleConfiguration configuration,
        IConsole console,
        IReporter reporter,
        ILogger<SampleCommand> logger
    )
    {
        logger.LogInformation("Starting {command}", GetType().Name);

        try
        {
            // TODO Validate command line arguments

            // TODO Your code here
            console.WriteLine("Hello world!");
            await Task.Yield(); // Simulate doing work

            // Note You can use the reporter to write to the console
            reporter.Warn("Sample console warning");
        }
        catch (Exception e)
        {

            logger.LogCritical(e, e.Message);
            console.WriteLine(e);
            return ExitCode.ApplicationError;
        }


        logger.LogInformation("Completed {command}", GetType().Name);

        return ExitCode.Success;
    }
}