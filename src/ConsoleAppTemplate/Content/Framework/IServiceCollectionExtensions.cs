using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConsoleAppTemplate.Framework;

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

                    // Get the section named by 'path' from the config file
                    .GetSection(path)

                    // Bind the section to the T class. This is reflection-based binding:
                    // it is fine for the default (framework-dependent, untrimmed) publish,
                    // but under a trimmed or single-file publish it raises SYSLIB1104 -
                    // switch to the configuration binder source generator
                    // (<EnableConfigurationBindingGenerator>true</EnableConfigurationBindingGenerator>)
                    // if you enable trimming.
                    .Get<T>()

                // If section is not found, throw an exception
                ?? throw new InvalidOperationException
                (
                    $"Could not bind to the config section '{path}'. " +
                    "Make sure the section exists in the config file and matches " +
                    "the specified class."
                )
        );

        return services;
    }
}
