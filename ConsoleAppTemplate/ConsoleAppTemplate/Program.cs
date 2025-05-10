// Remove or comment the following line to disable automatic command registration
#define AutomaticallyRegisterCommands


using System.Diagnostics;
using ConsoleAppTemplate.Command;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;


namespace ConsoleAppTemplate
{

    [Subcommand(typeof(Sub3Command))]
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            return await new HostBuilder()
                .RunCommandLineApplicationAsync<Program>(args, RegisterCommands);


        }



        private static void RegisterCommands(CommandLineApplication<Program> application)
        {
            RegisterCommandsWorker(application);
        }



        [Conditional("AutomaticallyRegisterCommands")]
        private static void RegisterCommandsWorker(CommandLineApplication<Program> application)
        {
            // Get all classes with the CommandAttribute
            var commandTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.GetCustomAttributes(typeof(CommandAttribute), inherit: true).Any())
                .ToList();

            foreach (var commandType in commandTypes)
            {
                application.Command(commandType.Name.ToLower(), config =>
                {

                    config.OnExecute(() =>
                    {
                        var instance = Activator.CreateInstance(commandType);
                        //var executeMethod = commandType.GetMethod("Execute");
                        //executeMethod?.Invoke(instance, null);
                        return 0;
                    });
                });
            }
        }
    }
}
