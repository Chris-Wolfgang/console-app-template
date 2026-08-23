using Xunit;

namespace ConsoleAppTemplate.Tests;

/// <summary>
/// Sample unit tests. These reach the app's <c>internal</c> types thanks to the
/// InternalsVisibleTo entry in the app csproj. Replace these with tests for your own
/// commands and services as you build them out.
/// </summary>
public class ExitCodeTests
{
    [Fact]
    public void Success_is_zero()
    {
        Assert.Equal(0, ExitCode.Success);
    }



    [Theory]
    [InlineData(ExitCode.UnhandledException)]
    [InlineData(ExitCode.ApplicationError)]
    [InlineData(ExitCode.Canceled)]
    public void Failure_codes_are_non_zero(int code)
    {
        Assert.NotEqual(ExitCode.Success, code);
    }
}
