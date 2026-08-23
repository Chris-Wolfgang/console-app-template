using AotConsoleTemplate.Model;
using Microsoft.Extensions.Options;

namespace AotConsoleTemplate.Services;

/// <summary>
/// Default <see cref="IGreeter"/>. Reads the greeting prefix from <see cref="SampleOptions"/>,
/// demonstrating strongly typed configuration flowing through dependency injection.
/// </summary>
public sealed class Greeter : IGreeter
{
    private readonly SampleOptions _options;



    /// <summary>Creates a <see cref="Greeter"/> from the bound <see cref="SampleOptions"/>.</summary>
    public Greeter(IOptions<SampleOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _options = options.Value;
    }



    /// <inheritdoc/>
    public string Greet(string name)
    {
        return $"{_options.Greeting}, {name}!";
    }
}
