using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;

namespace ConsoleAppTemplate.Framework
{
    internal static class CommandLineUtilsConfiguration
    {


        public static IHostBuilder AutomaticallyRegisterCommands(this IHostBuilder hostBuilder, CommandLineApplication app)
        {
            // Get the current assembly
            var assembly = Assembly.GetExecutingAssembly();

            // Find all types with the CommandAttribute
            var commands = assembly
                .GetTypes()
                .Where(type => type.GetCustomAttributes(typeof(CommandAttribute), inherit: true).Any())
                .Select((t => (Command: t, CommandAttribute:  t.GetCustomAttribute<CommandAttribute>())))
                .ToList();


            // Register each command type as a service
            foreach (var command in commands)
            {
                Console.WriteLine($"Registering '{command.Command.Name}' of type '{command.Command.FullName}'");

                //https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.Conventions.ConventionContext.html
                //https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.Conventions.SubcommandPropertyConvention.html

            }


            throw new NotImplementedException();
            return hostBuilder;
        }
    }
}
