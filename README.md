# Wolfgang Console App Templates

A robust set of .NET templates for building console applications with modern development best practices, including dependency injection, configuration management, structured logging, and extensible command-line parsing.

[![NuGet](https://img.shields.io/nuget/v/Wolfgang.Template.Console.svg?logo=nuget&label=NuGet)](https://www.nuget.org/packages/Wolfgang.Template.Console)
[![NuGet downloads](https://img.shields.io/nuget/dt/Wolfgang.Template.Console.svg?logo=nuget&label=downloads)](https://www.nuget.org/packages/Wolfgang.Template.Console)
[![PR build](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/console-app-template/pr.yaml?event=pull_request&label=PR%20build&logo=github)](https://github.com/Chris-Wolfgang/console-app-template/actions/workflows/pr.yaml)
[![Release](https://img.shields.io/github/actions/workflow/status/Chris-Wolfgang/console-app-template/release.yaml?label=release&logo=github)](https://github.com/Chris-Wolfgang/console-app-template/actions/workflows/release.yaml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![GitHub](https://img.shields.io/badge/GitHub-Repository-181717?logo=github)](https://github.com/Chris-Wolfgang/console-app-template)

---

## Table of Contents

- [Overview](#overview)
- [Templates in this Repository](#templates-in-this-repository)
- [Features](#features)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Configuration](#configuration)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Customization](#customization)
  - [Commands and Subcommands](#commands-and-subcommands)
  - [Configuration Files](#configuration-files)
  - [Logging](#logging)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

**Wolfgang.Template.Console** provides a solid foundation for .NET console applications. It leverages best practices such as structured logging (via Serilog), dependency injection, configuration via JSON files, and powerful command-line parsing (with McMaster.Extensions.CommandLineUtils).

The templates are designed for scalability, maintainability, and ease-of-use, whether for small scripts or complex automation tools.

---

## Templates in this Repository

| Template | Short name | Type | NuGet package |
|----------|------------|------|---------------|
| Wolfgang Console App | `cwconsole` | Project template — a complete console application | [Wolfgang.Template.Console](https://www.nuget.org/packages/Wolfgang.Template.Console) |
| Wolfgang Console Subcommand | `cwsubcmd` | Item template — adds a new subcommand class to an existing app | [Wolfgang.Template.Console.Subcommand](https://www.nuget.org/packages/Wolfgang.Template.Console.Subcommand) |
| Wolfgang Console ETL Subcommand | `cwsubcmdetl` | Item template — adds an ETL-style subcommand built on Wolfgang.Etl.Abstractions | [Wolfgang.Template.Console.ETL-SubCommand](https://www.nuget.org/packages/Wolfgang.Template.Console.ETL-SubCommand) |

---

## Features

- **Command Line Parsing:** Easily define commands, subcommands, and options.
- **Structured Logging:** Integrated Serilog with console and file sinks, plus support for enrichment.
- **Dependency Injection:** Built-in .NET DI with easy configuration.
- **Flexible Configuration:** Supports single or environment-specific JSON configuration files.
- **Error Handling:** Robust error catching and exit codes for integration in automation pipelines.
- **Analyzer Enforcement:** Generated projects ship with AsyncFixer, Meziantou, Roslynator, and Sonar analyzers enabled, with warnings treated as errors in Release builds.
- **Optional companion projects:** Generate with `--unit-tests`, `--integration-tests`, and/or `--benchmarks` to scaffold a matching xUnit unit-test project, an xUnit integration-test project, and a BenchmarkDotNet project — each referencing the app (with `InternalsVisibleTo` so internal types are reachable).
- **Optional OpenTelemetry:** Generate with `--otel` to wire OpenTelemetry tracing + metrics (console exporter by default; set `OpenTelemetry:OtlpEndpoint` in AppSettings to export via OTLP to Jaeger, Grafana, Azure Monitor, etc.).
- **Entry-point style:** Defaults to the classic `static Task<int> Main`; pass `--top-level` to generate a top-level-statements `Program.cs` instead (no explicit `Main`). Functionality is identical either way.
- **Extensible:** Easily add new commands, services, or configuration sections.

---

## Getting Started

### Prerequisites

- [.NET 8.0 SDK or later](https://dotnet.microsoft.com/en-us/download)
- (Optional) [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

Install the console application template from NuGet:

```sh
dotnet new install Wolfgang.Template.Console
```

Create a new project using the template:

```sh
dotnet new cwconsole -n MyConsoleApp
cd MyConsoleApp
```

To add more subcommands later, install the item templates as well:

```sh
dotnet new install Wolfgang.Template.Console.Subcommand
dotnet new install Wolfgang.Template.Console.ETL-SubCommand
```

**Review Next Steps:**
After creating your project, be sure to review the `Instructions.md` file included in the root of your new project directory. This file provides detailed, project-specific setup steps and guidance for customizing your application.
> If you create your project using Visual Studio, the `Instructions.md` file will open automatically to help guide you through initial configuration and customization.

Restore packages and build:

```sh
dotnet restore
dotnet build
```

### Configuration

The template uses JSON files for application and environment configuration.

- **AppSettings.json**: Main config file, used by default for all environments
- **AppSettings.Development.json, AppSettings.Production.json, etc.**: Environment-specific files

By default the generated app loads the single `AppSettings.json` file. To use one file per environment instead, change the `ConfigurationFileMethod.SingleFile` argument in `Program.cs` to `ConfigurationFileMethod.OneFilePerEnvironment`; the file is then selected by the `DOTNET_ENVIRONMENT` variable.

**To set the environment (example for Windows):**
```sh
set DOTNET_ENVIRONMENT=Development
```
Or for Linux/macOS:
```sh
export DOTNET_ENVIRONMENT=Development
```

---

## Usage

Run the application from the root directory:

```sh
dotnet run
```

You can also publish the app and run the executable:

```sh
dotnet publish -c Release
./bin/Release/net8.0/publish/MyConsoleApp.exe [options]
```

### Command Line Options

The template supports a main command and subcommands. To view help:

```sh
dotnet run -- --help
```

Add new subcommands by creating classes and registering them in `Program.cs` — or generate one with `dotnet new cwsubcmd`.

---

## Project Structure

A project generated from `cwconsole` looks like:

```
MyConsoleApp/
├── Program.cs
├── AppSettings.json
├── AppSettings.Development.json
├── AppSettings.Production.json
├── Command/
│   └── SampleCommand.cs
├── Framework/
│   └── (hosting, configuration, and console helpers)
├── Model/
│   └── SampleConfiguration.cs
├── Instructions.md
└── ...
```

- **Program.cs**: Entry point with main logic and configuration.
- **AppSettings*.json**: Application configuration files.
- **Command/**: Subcommand classes; add new commands here.
- **Framework/**: Hosting and configuration extension helpers.
- **Model/**: Configuration binding models.
- **Instructions.md**: In-depth development and customization notes.

---

## Customization

### Commands and Subcommands

- Define your main command in `Program.cs` using attributes.
- Add subcommands by creating new classes and registering them with `[Subcommand(typeof(MyCommand))]`.

Example:
```csharp
[Subcommand(typeof(MyNewCommand))]
```

### Configuration Files

- **Single File**: Use `AppSettings.json` for all environments (the default).
- **Per Environment**: Use `AppSettings.{Environment}.json` files by switching to `ConfigurationFileMethod.OneFilePerEnvironment`.

See [Instructions.md](src/ConsoleAppTemplate/Content/Instructions.md) for detailed setup.

### Logging

- Logging is configured via the `Serilog` section in your `AppSettings*.json`.
- Add or remove log sinks and adjust minimum levels as needed.
- Default logs to both console and file (see `AppSettings.json`).

---

## Security posture

This repo runs [OpenSSF Scorecard](https://scorecard.dev/) (weekly + on push to `main`)
to grade its security posture — branch protection, pinned dependencies, token
permissions, dangerous-workflow patterns, and more. Results upload as SARIF to
**Security → Code scanning**, alongside the other scanners (gitleaks, CodeQL, DevSkim,
InspectCode, and zizmor for workflow security).

- **Target score floor:** **≥ 7.0 / 10.** Investigate and address any check that
  drops the aggregate below that; the per-check breakdown is in Code Scanning.
- **Public badge / dashboard:** off by default (the score stays private in Code
  Scanning). To publish to the public OpenSSF dashboard and enable the badge below,
  set `publish_results: true` in [`.github/workflows/scorecard.yml`](.github/workflows/scorecard.yml)
  (and add `id-token: write` to that job) — a deliberate "make our posture public"
  decision. Once enabled, add:

  ```markdown
  [![OpenSSF Scorecard](https://api.securityscorecards.dev/projects/github.com/Chris-Wolfgang/console-app-template/badge)](https://scorecard.dev/viewer/?uri=github.com/Chris-Wolfgang/console-app-template)
  ```

See the [Threat Model](docs/THREAT-MODEL.md) for the STRIDE analysis behind these
controls, and [SECURITY.md](SECURITY.md) for how to report a vulnerability.

---

## Contributing

Contributions are welcome! See [CONTRIBUTING.md](CONTRIBUTING.md) for the full workflow, coding standards, and PR checklist.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## References & Further Reading

- [CHANGELOG](CHANGELOG.md) · [Migration Guide](docs/MIGRATION.md) (breaking template changes)
- [Serilog Documentation](https://serilog.net/)
- [McMaster.Extensions.CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)
- [Microsoft.Extensions.Hosting](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting)
- [.NET Generic Host](https://learn.microsoft.com/en-us/dotnet/core/extensions/generic-host)
- [Custom templates for dotnet new](https://learn.microsoft.com/en-us/dotnet/core/tools/custom-templates)

---

## Support

For questions, please open an issue on GitHub.

For more in-depth developer notes and template usage, see [src/ConsoleAppTemplate/Content/Instructions.md](src/ConsoleAppTemplate/Content/Instructions.md).
