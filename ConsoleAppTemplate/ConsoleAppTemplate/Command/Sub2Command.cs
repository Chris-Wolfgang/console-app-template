using McMaster.Extensions.CommandLineUtils;

namespace ConsoleAppTemplate.Command;

[Command]
internal class Sub2Command
{
    public Task<int> OnExecuteAsync()
    {
        Console.WriteLine("SubCommand2 executed");
        return Task.FromResult(0);
    }
}