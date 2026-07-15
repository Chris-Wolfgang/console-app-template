using ConsoleAppTemplate.Framework;
using ConsoleAppTemplate.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ConsoleAppTemplate.IntegrationTests;

/// <summary>
/// Integration tests exercise the app's real composition rather than a single unit in
/// isolation. This one drives the app's own <see cref="IServiceCollectionExtensions"/>
/// binding helper against a live configuration + DI container to prove a config section
/// flows through to a strongly-typed options object. Grow this suite with tests that run
/// commands through the host, hit a test database, or call a stubbed downstream service.
/// </summary>
public class ConfigurationBindingTests
{
    [Fact]
    public void BindConfigSection_binds_the_section_to_a_typed_object()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["SampleConfiguration:CommandTimeout"] = "42",
            })
            .Build();

        using var provider = new ServiceCollection()
            .AddSingleton<IConfiguration>(configuration)
            .BindConfigSection<SampleConfiguration>("SampleConfiguration")
            .BuildServiceProvider();

        var bound = provider.GetRequiredService<SampleConfiguration>();

        Assert.Equal(42, bound.CommandTimeout);
    }
}
