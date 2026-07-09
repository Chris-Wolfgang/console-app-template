# Third-Party Notices

Projects generated from the templates in this repository reference the following third-party packages. Each package is owned by its respective authors and licensed under its own terms; the licenses below are as published on NuGet at the time of writing.

## Wolfgang.Template.Console (`cwconsole`)

| Package | Purpose | License |
|---------|---------|---------|
| [McMaster.Extensions.CommandLineUtils](https://www.nuget.org/packages/McMaster.Extensions.CommandLineUtils) | Command-line parsing (commands, subcommands, options) | Apache-2.0 |
| [McMaster.Extensions.Hosting.CommandLine](https://www.nuget.org/packages/McMaster.Extensions.Hosting.CommandLine) | Generic-host integration for CommandLineUtils | Apache-2.0 |
| [Microsoft.Extensions.Hosting](https://www.nuget.org/packages/Microsoft.Extensions.Hosting) | Generic host, dependency injection, configuration | MIT |
| [System.Configuration.ConfigurationManager](https://www.nuget.org/packages/System.Configuration.ConfigurationManager) | Configuration types | MIT |
| [Serilog](https://www.nuget.org/packages/Serilog) | Structured logging | Apache-2.0 |
| [Serilog.Exceptions](https://www.nuget.org/packages/Serilog.Exceptions) | Exception detail enrichment | MIT |
| [Serilog.Extensions.Hosting](https://www.nuget.org/packages/Serilog.Extensions.Hosting) | Serilog ↔ generic host integration | Apache-2.0 |
| [Serilog.Extensions.Logging](https://www.nuget.org/packages/Serilog.Extensions.Logging) | Serilog provider for Microsoft.Extensions.Logging | Apache-2.0 |
| [Serilog.Settings.Configuration](https://www.nuget.org/packages/Serilog.Settings.Configuration) | Serilog configuration from AppSettings.json | Apache-2.0 |
| [Serilog.Sinks.Console](https://www.nuget.org/packages/Serilog.Sinks.Console) | Console log sink | Apache-2.0 |
| [Serilog.Sinks.File](https://www.nuget.org/packages/Serilog.Sinks.File) | Rolling-file log sink | Apache-2.0 |

### Analyzers (build-time only, `PrivateAssets=all` — not shipped with the application)

| Package | License |
|---------|---------|
| [AsyncFixer](https://www.nuget.org/packages/AsyncFixer) | MIT |
| [Meziantou.Analyzer](https://www.nuget.org/packages/Meziantou.Analyzer) | MIT |
| [Roslynator.Analyzers](https://www.nuget.org/packages/Roslynator.Analyzers) | Apache-2.0 |
| [SonarAnalyzer.CSharp](https://www.nuget.org/packages/SonarAnalyzer.CSharp) | LGPL-3.0-only |

## Wolfgang.Template.Console.Subcommand (`cwsubcmd`)

Adds a class that uses the packages already referenced by the host application (McMaster.Extensions.CommandLineUtils, Microsoft.Extensions.Logging); it introduces no additional package references.

## Wolfgang.Template.Console.ETL-SubCommand (`cwsubcmdetl`)

| Package | Purpose | License |
|---------|---------|---------|
| [Wolfgang.Etl.Abstractions](https://www.nuget.org/packages/Wolfgang.Etl.Abstractions) | ETL extract/transform/load abstractions | MIT |

Plus the host application's existing packages, as with `cwsubcmd`.
