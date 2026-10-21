using ConsoleAppTemplate.Framework;
using Xunit;

namespace ConsoleAppTemplate.Tests;

/// <summary>
/// Sample unit tests. These reach the app's <c>internal</c> types thanks to the
/// InternalsVisibleTo entry in the app csproj. Replace these with tests for your own
/// commands and services as you build them out.
/// </summary>
public class AutoRegisterCommandsConventionTests
{
    [Theory]
    [InlineData("SampleCommand", "sample")]
    [InlineData("HealthCommand", "health")]
    [InlineData("ImportCsvCommand", "importcsv")]
    public void DeriveName_when_type_name_ends_with_Command_strips_suffix_and_lower_cases(string typeName, string expected)
    {
        var actual = AutoRegisterCommandsConvention.DeriveName(typeName);

        Assert.Equal(expected, actual);
    }



    [Theory]
    [InlineData("Export", "export")]
    [InlineData("Command", "command")]
    public void DeriveName_when_type_name_has_no_Command_suffix_lower_cases_the_whole_name(string typeName, string expected)
    {
        var actual = AutoRegisterCommandsConvention.DeriveName(typeName);

        Assert.Equal(expected, actual);
    }
}
