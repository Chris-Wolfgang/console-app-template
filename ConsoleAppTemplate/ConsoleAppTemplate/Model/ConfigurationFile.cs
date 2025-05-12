namespace ConsoleAppTemplate.Model;

internal record ConfigurationFile
{
    public required string Name { get; init; }
    public bool Optional { get; init; }
    public bool ReloadOnChange { get; init; }

}