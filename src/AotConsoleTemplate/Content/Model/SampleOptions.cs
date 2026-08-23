namespace AotConsoleTemplate.Model;

/// <summary>
/// Strongly typed view of the "Sample" section of appsettings.json. Bound in Program.cs
/// with the source-generated configuration binder so it stays AOT- and trim-safe.
/// </summary>
public sealed class SampleOptions
{
    /// <summary>The greeting prefix, e.g. "Hello". Configured under Sample:Greeting.</summary>
    public string Greeting { get; set; } = "Hello";
}
