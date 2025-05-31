using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppTemplate.Framework
{
    // ReSharper disable once InconsistentNaming
    internal static class IServiceCollectionExtensions
    {
        internal static IServiceCollection BindConfigSection<T> 
        (
            this IServiceCollection services,
            string path
        ) where T : class, new()
        {
            services.AddSingleton
            (
                provider => provider
                        // Get the configuration from the host builder
                        .GetRequiredService<IConfiguration>()

                        // Get the SampleConfiguration section from the config file
                        .GetSection(path)

                        // Bind the section to the T class
                        .Get<T>()

                    // If section is not found, throw an exception
                    ?? throw new ConfigurationErrorsException
                    (
                        $"Could not bind to the config section '{path}'. " +
                        "Make sure the section exists in the config file and matches " +
                        "the specified class."
                    )
            );

            return services;
        }
    }
}
