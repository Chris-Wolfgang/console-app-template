using System.CommandLine;
using AotConsoleTemplate.Model;
using AotConsoleTemplate.Services;
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

// Route every log line to stderr so stdout carries only the command's output and the app
// composes in pipelines (`app | jq`). The default console logger writes to stdout.
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

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

// Resolved here rather than inside the action so the lambda never captures `host`, which the
// enclosing `using` disposes. Nothing is disposed early as written - the action runs before
// the scope ends - but capturing a disposable in a callback is not a pattern worth teaching
// in a template (ReSharper: AccessToDisposedClosure).
var greeter = host.Services.GetRequiredService<IGreeter>();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

rootCommand.SetAction(async (parseResult, cancellationToken) =>
{
    var name = parseResult.GetValue(nameOption)!;

    logger.LogInformation("Greeting {Name}", name);
    await Console.Out.WriteLineAsync(greeter.Greet(name).AsMemory(), cancellationToken);

    return 0;
});

// Start the host so any IHostedService you register actually runs for the lifetime of the
// command, and stop it afterwards so those services shut down cleanly before disposal.
await host.StartAsync();
try
{
    return await rootCommand.Parse(args).InvokeAsync();
}
finally
{
    await host.StopAsync();
}
