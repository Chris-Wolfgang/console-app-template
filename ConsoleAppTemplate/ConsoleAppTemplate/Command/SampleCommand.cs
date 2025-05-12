using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;

namespace ConsoleAppTemplate.Command;

[Command
(
    // TODO You can override the default name for the command by uncommenting the line below
    //Name = "<override with command if needed>",

    // TODO Add a description for your command
    Description = "<add description here>"
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
            Description = "If set, the program will not send an email when complete"
        )
    ]
    [EmailAddress]
    [MaxLength(255)]
    public string? NotifyUponComplete { get; set; }
}