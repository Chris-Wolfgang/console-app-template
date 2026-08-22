namespace ConsoleAppTemplate.Model;

internal record SampleConfiguration
{
    // The setter is invoked by Microsoft.Extensions.Configuration binding via
    // reflection (see the BindConfigSection<SampleConfiguration> call in
    // Program.cs). `init` matches the immutable-record semantics we want:
    // settable during construction only, then read-only. InspectCode still
    // classifies init as a "settable accessor" for the unused-accessor rule,
    // so a narrow inline disable rides along.
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public int CommandTimeout { get; init; }


}