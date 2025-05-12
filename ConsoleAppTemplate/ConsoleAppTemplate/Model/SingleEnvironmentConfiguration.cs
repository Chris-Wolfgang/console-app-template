namespace ConsoleAppTemplate.Model;

internal record SingleEnvironmentConfiguration
{
    public required string BasePath { get; init; }

    public required ConfigurationFile ConfigFile { get; init; }
}