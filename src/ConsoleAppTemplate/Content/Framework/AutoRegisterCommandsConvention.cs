using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Conventions;

namespace ConsoleAppTemplate.Framework;

/// <summary>
/// Discovers every class in this assembly decorated with <see cref="CommandAttribute"/>
/// (other than the root <see cref="Program"/> class itself) and registers it as a
/// subcommand of the root application, so new commands (for example ones added with
/// <c>dotnet new cwsubcmd</c>) work without editing Program.cs.
/// </summary>
/// <remarks>
/// The subcommand name is taken from <see cref="CommandAttribute.Name"/> when set;
/// otherwise it is derived from the class name by removing a trailing "Command" and
/// lower-casing the remainder (SampleCommand becomes "sample"). Commands already
/// registered another way (for example via a [Subcommand] attribute on Program) are
/// skipped, so explicit and automatic registration can coexist.
/// </remarks>
internal class AutoRegisterCommandsConvention : IConvention
{
    // The Register method is public (on this internal class) so the reflection
    // lookup below needs no accessibility bypass (S3011).
    private static readonly MethodInfo RegisterMethod = typeof(AutoRegisterCommandsConvention)
        .GetMethod(nameof(Register), BindingFlags.Public | BindingFlags.Static)!;



    public void Apply(ConventionContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Only apply to the root command; without this guard the convention would
        // re-run for every subcommand it registers and recurse forever.
        if (context.ModelType != typeof(Program))
        {
            return;
        }

        var commandTypes = typeof(Program).Assembly
            .GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false } && type != typeof(Program))
            .Select(type => (Type: type, Attribute: type.GetCustomAttribute<CommandAttribute>()))
            .Where(candidate => candidate.Attribute is not null);

        foreach (var (type, attribute) in commandTypes)
        {
            // An empty or whitespace Name on the attribute would register an
            // unusable command - fall back to the derived name instead.
            var name = string.IsNullOrWhiteSpace(attribute!.Name)
                ? DeriveName(type.Name)
                : attribute.Name;

            if (context.Application.Commands.Any(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            RegisterMethod
                .MakeGenericMethod(type)
                .Invoke(null, [context.Application, name]);
        }
    }



    /// <summary>
    /// Derives the default subcommand name from a class name: a trailing "Command"
    /// is removed and the remainder is lower-cased, e.g. SampleCommand -> "sample".
    /// </summary>
    private static string DeriveName(string typeName)
    {
        const string suffix = "Command";

        var trimmed = typeName.EndsWith(suffix, StringComparison.Ordinal) && typeName.Length > suffix.Length
            ? typeName[..^suffix.Length]
            : typeName;

        return trimmed.ToLowerInvariant();
    }



    public static void Register<TCommand>(CommandLineApplication application, string name) where TCommand : class
    {
        ArgumentNullException.ThrowIfNull(application);

        application.Command<TCommand>(name, _ => { });
    }
}
