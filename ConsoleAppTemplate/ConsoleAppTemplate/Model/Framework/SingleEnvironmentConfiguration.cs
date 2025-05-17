namespace ConsoleAppTemplate.Model.Framework;

internal record SingleEnvironmentConfiguration
{
    public required string BasePath { get; init; }

    public required ConfigurationFile ConfigFile { get; init; }
}