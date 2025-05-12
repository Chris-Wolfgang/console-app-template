using ConsoleAppTemplate.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace ConsoleAppTemplate.Framework
{
    internal static class IHostBuilderExtensions
    {
        public static IHostBuilder UseSingleEnvironment(this IHostBuilder builder)
        {
            var config = new SingleEnvironmentConfiguration
            {
                BasePath = ConfigurationPath.Combine(AppContext.BaseDirectory),
                ConfigFile = new ConfigurationFile
                {
                    Name = "AppSettings.json",
                    Optional = false,
                    ReloadOnChange = true
                }
            };

            return UseSingleEnvironment(builder, config);
        }



        public static IHostBuilder UseSingleEnvironment(this IHostBuilder builder, SingleEnvironmentConfiguration config)
         {
            builder
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder
                        .SetBasePath(config.BasePath)
                        .AddJsonFile(config.ConfigFile.Name, optional: config.ConfigFile.Optional,
                            reloadOnChange: config.ConfigFile.ReloadOnChange)
                        .AddEnvironmentVariables();
                });

            return builder;
        }



        public static IHostBuilder UseMultiEnvironment(this IHostBuilder builder)
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(environment))
            {
                Environment.FailFast("System variable DOTNET_ENVIRONMENT is not set.");
            }

            var config = new MultiEnvironmentConfiguration
            {
                BasePath = ConfigurationPath.Combine(AppContext.BaseDirectory),
                ConfigFile = new ConfigurationFile
                {
                    Name = $"AppSettings.{environment}.json",
                    Optional = false,
                    ReloadOnChange = true
                }
            };

            return UseMultiEnvironment(builder, config);
        }



        public static IHostBuilder UseMultiEnvironment(this IHostBuilder builder, MultiEnvironmentConfiguration config)
        {
            builder
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder
                        .SetBasePath(config.BasePath)
                        .AddJsonFile(config.ConfigFile.Name, optional: config.ConfigFile.Optional,
                            reloadOnChange: config.ConfigFile.ReloadOnChange)
                        .AddEnvironmentVariables();
                });

            return builder;
        }



    }
}
