namespace ConsoleAppTemplate.Model;

internal record MultiEnvironmentConfiguration
{
    public required string BasePath { get; init; }

    public required ConfigurationFile ConfigFile { get; init; }
}