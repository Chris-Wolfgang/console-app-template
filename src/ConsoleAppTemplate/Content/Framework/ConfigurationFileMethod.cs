namespace ConsoleAppTemplate.Framework;

/// <summary>
/// Specifies how configuration files are loaded when calling
/// <see cref="IHostBuilderExtensions.AddConfigurationFile"/>.
/// </summary>
internal enum ConfigurationFileMethod
{
    /// <summary>
    /// All environments share a single AppSettings.json file.
    /// </summary>
    SingleFile,

    /// <summary>
    /// Each environment loads its own AppSettings.{environment}.json file,
    /// selected by the DOTNET_ENVIRONMENT variable.
    /// </summary>
    OneFilePerEnvironment
}
