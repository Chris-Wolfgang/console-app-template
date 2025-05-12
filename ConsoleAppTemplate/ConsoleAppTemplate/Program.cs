using ConsoleAppTemplate.Command;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace ConsoleAppTemplate
{

    [Command
    (
        // TODO You can override the default name for the command by uncommenting the line below
        Name = "ConsoleAppTemplate",

        // TODO Add a description for the command
        Description = "A template for a console application complete with command line parse, logging, DI and more.",

        // TODO Uncomment this line to make this app support response files. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.ResponseFileHandling.html
        // ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated

        // TODO Specify what to do with unrecognized arguments. The default is to throw an exception. See https://natemcmaster.github.io/CommandLineUtils/v3.0/api/McMaster.Extensions.CommandLineUtils.UnrecognizedArgumentHandling.html
        UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.Throw

    )
    ]
    [Subcommand(typeof(SampleCommand))]
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            return await new HostBuilder()
                .ConfigureServices((context, collection) =>
                {
                    collection.AddSingleton<IReporter, ConsoleReporter>();
                })
                .RunCommandLineApplicationAsync<Program>(args);
        }



        /// <summary>
        /// This method is called if the user does not specify a sub command
        /// </summary>
        /// <param name="application"></param>
        /// <param name="reporter"></param>
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
            IReporter reporter
        )
        {
            // TODO if you are using not using sub commands then you can remove the lines below and replace with your own code
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
