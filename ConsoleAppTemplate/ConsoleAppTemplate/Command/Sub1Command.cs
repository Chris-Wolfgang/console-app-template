using McMaster.Extensions.CommandLineUtils;

namespace ConsoleAppTemplate.Command;

[Command]
internal class Sub1Command
{
    public void Execute()
    {
        Console.WriteLine("SubCommand1 executed");
    }
}