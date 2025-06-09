# ConsoleAppTemplate

A robust template for building .NET console applications with modern development best practices, including dependency injection, configuration management, logging, and extensible command-line parsing.

---

## Table of Contents

- [Overview](#overview)
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

**ConsoleAppTemplate** provides a solid foundation for .NET console applications. It leverages best practices such as structured logging (via Serilog), dependency injection, configuration via JSON files, and powerful command-line parsing (with McMaster.Extensions.CommandLineUtils).

This template is designed for scalability, maintainability, and ease-of-use, whether for small scripts or complex automation tools.

---

## Features

- **Command Line Parsing:** Easily define commands, subcommands, and options.
- **Structured Logging:** Integrated Serilog with console and file sinks, plus support for enrichment.
- **Dependency Injection:** Built-in .NET DI with easy configuration.
- **Flexible Configuration:** Supports single or environment-specific JSON configuration files.
- **Error Handling:** Robust error catching and exit codes for integration in automation pipelines.
- **Extensible:** Easily add new commands, services, or configuration sections.

---

## Getting Started

### Prerequisites

- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/en-us/download)
- (Optional) [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

Install the template from NuGet:

```sh
dotnet new install <PackageID>
```
> Replace `<PackageID>` with the actual NuGet package identifier for ConsoleAppTemplate.

Create a new project using this template:

```sh
dotnet new console-app-template -n MyConsoleApp
cd MyConsoleApp
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

- **appsettings.json**: Main config file (default for all environments)
- **appsettings.Development.json, appsettings.Production.json, etc.**: Environment-specific files (optional)

The configuration system will load the appropriate file based on the `DOTNET_ENVIRONMENT` variable.

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
./bin/Release/net6.0/MyConsoleApp.exe [options]
```

### Command Line Options

The template supports a main command and subcommands. To view help:

```sh
dotnet run -- --help
```

Add new subcommands by creating classes and registering them in `Program.cs`.

---

## Project Structure

```
MyConsoleApp/
├── Program.cs
├── AppSettings.json
├── AppSettings.Development.json
├── AppSettings.Production.json
├── Instructions.md
└── ...
```

- **Program.cs**: Entry point with main logic and configuration.
- **AppSettings*.json**: Application configuration files.
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

- **Single File**: Use `appsettings.json` for all environments.
- **Per Environment**: Use `appsettings.{Environment}.json` files.

See [Instructions.md](src/ConsoleAppTemplate/Content/Instructions.md) for detailed setup.

### Logging

- Logging is configured via the `Serilog` section in your `AppSettings*.json`.
- Add or remove log sinks and adjust minimum levels as needed.
- Default logs to both console and file (see `AppSettings.json`).

---

## Contributing

Contributions are welcome! Please fork the repo and submit a pull request for improvements or bug fixes.

1. Fork the repository
2. Create a new branch (`git checkout -b feature/my-feature`)
3. Commit your changes (`git commit -am 'Add new feature'`)
4. Push to the branch (`git push origin feature/my-feature`)
5. Open a pull request

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## References & Further Reading

- [Serilog Documentation](https://serilog.net/)
- [McMaster.Extensions.CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)
- [Microsoft.Extensions.Hosting](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting)
- [.NET Generic Host](https://docs.microsoft.com/en-us/dotnet/core/extensions/generic-host)

---

## Support

For questions, please open an issue on GitHub.

```
For more in-depth developer notes and template usage, see [src/ConsoleAppTemplate/Content/Instructions.md](src/ConsoleAppTemplate/Content/Instructions.md).
```