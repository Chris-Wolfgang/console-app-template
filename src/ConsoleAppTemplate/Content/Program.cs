﻿using System.Reflection;
using ConsoleAppTemplate.Command;
using ConsoleAppTemplate.Framework;
using ConsoleAppTemplate.Model;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;


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
    [Subcommand(typeof(SampleCommand))]
    // TODO Add additional sub commands here
    //[Subcommand(typeof(<your type>))]
    internal class Program
    {
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
                    
                    // Configure dependency injection
                    .ConfigureServices((context, serviceCollection) =>
                    {
                        serviceCollection
                            .AddSingleton<IReporter, ConsoleReporter>()

                            // TODO Remove SampleConfiguration
                            .BindConfigSection<SampleConfiguration>("SampleConfiguration")

                            // TODO Add additional services here

                            ;
                    })
                    .RunCommandLineApplicationAsync<Program>(args);
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

            var ex = new NotImplementedException("Test");

            for (var i = 0; i < 10_000_000; ++i)
            {
                logger.Verbose(ex, "Verbose");
                logger.Debug(ex, "Debug");
                logger.Information(ex, "Information");
                logger.Warning(ex, "Warning");
                logger.Error(ex, "Error");
                logger.Fatal(ex, "Fatal");
            }

            reporter.Output($"\nCommandTimeout from config file: {sampleConfiguration.CommandTimeout}\n\n");

            // TODO if you are not using sub commands then you can remove the lines below and replace with your own code
            application.ShowHelp();
            return ExitCode.Success;


            // TODO If you are using sub commands then you can remove the lines below and use the code above
            // TODO Add your code here
            reporter.Warn("Your code here");


            // TODO Return 0 for success and any positive number for failure
            return ExitCode.Success;
        }
    }
}
