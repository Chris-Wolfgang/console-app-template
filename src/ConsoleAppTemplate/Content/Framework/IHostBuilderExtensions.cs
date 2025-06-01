using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConsoleAppTemplate.Framework
{
    internal enum ConfigurationFileMethod
    {
        SingleFile,
        OneFilePerEnvironment
    }



    internal static class IHostBuilderExtensions
    {
        /// <summary>
        /// Adds a configuration file to the host builder.
        /// </summary>
        /// <param name="builder"> </param>
        /// <param name="method"> </param>
        /// <param name="optional" ></param>
        /// <param name="reloadOnChange"> </param>
        /// <returns>IHostBuilder</returns>
        /// <exception cref="ArgumentNullException">builder is null</exception>
        /// <exception cref="ArgumentOutOfRangeException">method was not a valid value</exception>
        public static IHostBuilder AddConfigurationFile
        (
            this IHostBuilder builder, 
            ConfigurationFileMethod method,
            bool optional = false,
            bool reloadOnChange = false
        )
        {
            ArgumentNullException.ThrowIfNull(builder);

            return method switch
            {
                ConfigurationFileMethod.SingleFile => AddSingleConfigFile(builder, optional, reloadOnChange),
                ConfigurationFileMethod.OneFilePerEnvironment => AddConfigFileForEnvironment(builder, optional, reloadOnChange),
                _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
            };
        }



        private static IHostBuilder AddSingleConfigFile
        (
            this IHostBuilder builder,
            bool optional,
            bool reloadOnChange 
        )
         {
            builder
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder
                        .SetBasePath(ConfigurationPath.Combine(AppContext.BaseDirectory))
                        .AddJsonFile("AppSettings.json", optional, reloadOnChange)
                        .AddEnvironmentVariables();
                });

            return builder;
        }



        private static IHostBuilder AddConfigFileForEnvironment
            (
                this IHostBuilder builder,
                bool optional,
                bool reloadOnChange
            )
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(environment))
            {
                Environment.FailFast("System variable DOTNET_ENVIRONMENT is not set.");
            }

            builder
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder
                        .SetBasePath(ConfigurationPath.Combine(AppContext.BaseDirectory))
                        .AddJsonFile($"AppSettings.{environment}.json", optional, reloadOnChange)
                        .AddEnvironmentVariables();
                });
            return builder;
        }
    }
}
