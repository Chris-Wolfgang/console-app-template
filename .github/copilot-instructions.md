# Copilot Coding Agent Instructions

## Repository Summary

This is a **.NET template pack** repository that provides `dotnet new` templates for creating console applications with subcommand support. It packages three templates as NuGet packages:

- **Wolfgang.Template.Console** — Console application template with logging, configuration, and command-line parsing
- **Wolfgang.Template.Console.Subcommand** — Subcommand template for adding commands to a console app
- **Wolfgang.Template.Console.ETL-SubCommand** — ETL-specific subcommand template with Wolfgang.Etl.Abstractions integration

**Repository Type**: Template pack (NuGet packages containing `dotnet new` templates)
**Primary Language**: C#
**Key Dependencies**: McMaster.Extensions.CommandLineUtils, Serilog, Wolfgang.Etl.Abstractions

## Build and Validation Instructions

### Prerequisites
- .NET SDK (8.0+ recommended)
- DevSkim CLI (installed via `dotnet tool install --global Microsoft.CST.DevSkim.CLI`)

### Build Process

1. **Build all template pack projects**:
   ```powershell
   dotnet build src/ConsoleAppTemplate/dotnet.consoleapp.template.pack.csproj --configuration Release
   dotnet build src/SubCommandTemplate/dotnet.subcommand.template.pack.csproj --configuration Release
   dotnet build src/EtlSubCommandTemplate/dotnet.etl-subcommand.template.pack.csproj --configuration Release
   ```

2. **Build template content project** (validates template code compiles):
   ```powershell
   dotnet build src/ConsoleAppTemplate/Content/ConsoleAppTemplate.csproj --configuration Release
   ```

3. **Pack NuGet packages**:
   ```powershell
   dotnet pack src/ConsoleAppTemplate/dotnet.consoleapp.template.pack.csproj --configuration Release --output ./nuget-packages
   dotnet pack src/SubCommandTemplate/dotnet.subcommand.template.pack.csproj --configuration Release --output ./nuget-packages
   dotnet pack src/EtlSubCommandTemplate/dotnet.etl-subcommand.template.pack.csproj --configuration Release --output ./nuget-packages
   ```

### Important Notes
- This repo has **no test projects** — it contains only template definitions
- Template content is in `src/*/Content/` directories
- Template configuration is in `.template.config/template.json` and `ide.host.json` files
- Version numbers are in `<PackageVersion>` tags in the `.csproj` files — keep all three in sync

## Project Layout

```
root/
├── src/
│   ├── ConsoleAppTemplate/           # Console app template pack
│   │   ├── Content/                  # Template source code
│   │   │   ├── .template.config/     # Template configuration
│   │   │   └── *.cs                  # Template C# files
│   │   └── dotnet.consoleapp.template.pack.csproj
│   ├── SubCommandTemplate/           # Subcommand template pack
│   │   ├── Content/
│   │   │   └── .template.config/
│   │   └── dotnet.subcommand.template.pack.csproj
│   └── EtlSubCommandTemplate/       # ETL subcommand template pack
│       ├── Content/
│       │   └── .template.config/
│       └── dotnet.etl-subcommand.template.pack.csproj
├── docfx_project/                    # DocFX documentation
└── .github/                          # GitHub configuration
```

## Agent Guidelines

### When Working with This Repository
1. **Template JSON files** (`template.json`, `ide.host.json`) follow the .NET template engine schema
2. **Symbol replacements** in template.json define parameters users pass to `dotnet new`
3. **PackageVersion** must stay in sync across all three `.csproj` files
4. **No tests exist** — validate changes by building all projects successfully
5. **Security**: Run DevSkim scan before submitting changes
