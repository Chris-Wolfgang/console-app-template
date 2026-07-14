using System.Reflection;
using ConsoleAppTemplate.Command;
using ConsoleAppTemplate.Framework;
using ConsoleAppTemplate.Model;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace ConsoleAppTemplate
{

    [Command
    (
        // TODO You can override the default name for the command by uncommenting the line below
        Name = "ConsoleAppTemplate",

        // TODO Add a description for the command
        Description = "A template for a console application complete with command line parse, logging, DI and more.",

        // TODO Specify what to do with unrecognized arguments. The default is to throw an exception. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.UnrecognizedArgumentHandling.html
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw,

        // TODO Specify response file handling. Default is disabled. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.ResponseFileHandling.html
        ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated
    )]
    // Sub commands are discovered and registered automatically - every class in this
    // assembly decorated with [Command], other than this Program class itself, becomes
    // a subcommand (see Framework/AutoRegisterCommandsConvention.cs). Just add a new
    // command class (e.g. via 'dotnet new cwsubcmd') and it is picked up on the next run.
    // TODO If you prefer explicit registration, remove the AddConvention call in Main
    // and list each command here instead:
    // [Subcommand(typeof(SampleCommand))]
    [VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
    internal class Program
    {
        /// <summary>
        /// Supplies the value shown by the --version option. Reads the assembly's
        /// informational version (set via the csproj Version/PackageVersion), falling
        /// back to the assembly version when no informational version is present.
        /// </summary>
        private static string GetVersion()
        {
            var assembly = Assembly.GetEntryAssembly();

            return assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly?.GetName().Version?.ToString()
                ?? "unknown";
        }




        private static async Task<int> Main(string[] args)
        {
            try
            {
                // Create a new HostBuilder to build the application
                return await new HostBuilder()
                    .AddConfigurationFile
                        (
                            // TODO If using separate config file per environment change this to OneFilePerEnvironment
                            ConfigurationFileMethod.SingleFile, 
                            optional: false,
                            reloadOnChange: false
                        )

                    // UseSerilog
                    .UseSerilog((context, configuration) =>
                    {
                        configuration
                            .ReadFrom.Configuration(context.Configuration)
                            .Enrich.WithProperty("Version", Assembly.GetEntryAssembly()?.GetName().Version)
                            ;
                    })
                    
#if (otel)
                    // OpenTelemetry tracing + metrics (see Framework/OpenTelemetryHostBuilderExtensions).
                    .ConfigureApplicationTelemetry()
#endif

                    // Configure dependency injection
                    .ConfigureServices((_, serviceCollection) =>
                    {
                        serviceCollection
                            .AddSingleton<IReporter, ConsoleReporter>()

                            // TODO Remove SampleConfiguration
                            .BindConfigSection<SampleConfiguration>("SampleConfiguration")

                            // TODO Add additional services here

                            ;
                    })
                    .RunCommandLineApplicationAsync<Program>
                    (
                        args,
                        application => application.Conventions.AddConvention(new AutoRegisterCommandsConvention())
                    );
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.Message);
                Log.Logger.Fatal(e, "Unhandled exception: {Message}", e.Message);
                return ExitCode.UnhandledException;
            }
            finally
            {
                await Log.CloseAndFlushAsync();
            }
        }



        /// <summary>
        /// This method is called if the user does not specify a sub command
        /// </summary>
        /// <param name="application"></param>
        /// <param name="reporter"></param>
        /// <param name="sampleConfiguration"></param>
        /// <param name="logger"></param>
        /// <returns>0 on success or any positive number for failure</returns>
        /// <remarks>
        /// - If you are not using sub commands you can rewrite this method to meet your needs
        /// - You can add and remove any parameters as needed, but you will need to configure dependency injection
        /// - If you modify this method to do async work, it is recommended to change the signature to
        ///   Task&lt;int&gt; OnExecuteAsync
        /// </remarks>
        internal int OnExecute
        (
            CommandLineApplication<Program> application,
            SampleConfiguration sampleConfiguration,
            IReporter reporter,
            ILogger logger
        )
        {

            logger.LogDebug("Starting {Command}", GetType().Name);

            // TODO Remove along with SampleConfiguration - demonstrates reading bound configuration
            logger.LogDebug("CommandTimeout is {CommandTimeout}", sampleConfiguration.CommandTimeout);


            // TODO if you are not using sub commands then you can remove the
            // three lines below
            reporter.Warn("No sub command specified");
            application.ShowHelp();
            return ExitCode.Success;


            // TODO If you are not using sub commands then you can remove the
            // lines above and add your code here, e.g.
            //
            //     reporter.Warn("Your code here");
            //
            //     // Return 0 for success and any positive number for failure
            //     return ExitCode.Success;
        }
    }
}
