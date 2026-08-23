using System.CommandLine;
using AotConsoleTemplate.Model;
using AotConsoleTemplate.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// A native-AOT-ready console app. Argument parsing uses System.CommandLine (no
// reflection), configuration binding uses the source-generated binder, and everything
// runs through the generic host for logging + dependency injection. Publish a native
// executable with, for example:
//
//     dotnet publish -c Release -r linux-x64
//
// The host builder loads appsettings.json (+ environment variables + command line).
var builder = Host.CreateApplicationBuilder(args);

// Bind the "Sample" section to strongly typed options. EnableConfigurationBindingGenerator
// (see the csproj) makes this AOT- and trim-safe by source-generating the binding.
builder.Services.Configure<SampleOptions>(builder.Configuration.GetSection("Sample"));

// TODO Register your own services here.
builder.Services.AddSingleton<IGreeter, Greeter>();

using var host = builder.Build();

// TODO Define your CLI surface here - add options, arguments and subcommands.
// See https://learn.microsoft.com/dotnet/standard/commandline/ for the full API.
var nameOption = new Option<string>("--name", "-n")
{
    Description = "The name to greet.",
    DefaultValueFactory = _ => "world",
};

var rootCommand = new RootCommand("A native-AOT-ready console application built with System.CommandLine.")
{
    nameOption,
};

rootCommand.SetAction(async (parseResult, cancellationToken) =>
{
    var name = parseResult.GetValue(nameOption)!;

    var greeter = host.Services.GetRequiredService<IGreeter>();
    var logger = host.Services.GetRequiredService<ILogger<Program>>();

    logger.LogInformation("Greeting {Name}", name);
    await Console.Out.WriteLineAsync(greeter.Greet(name).AsMemory(), cancellationToken);

    return 0;
});

return await rootCommand.Parse(args).InvokeAsync();
