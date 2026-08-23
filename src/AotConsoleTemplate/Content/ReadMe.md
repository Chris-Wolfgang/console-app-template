# {description}

A native-AOT-ready console application generated from the Wolfgang **cwconsole-aot** template.

## Why this variant

Unlike the reflection-based `cwconsole` template, everything here is compatible with
[Native AOT](https://learn.microsoft.com/dotnet/core/deploying/native-aot/):

- **Argument parsing:** [System.CommandLine](https://learn.microsoft.com/dotnet/standard/commandline/) — no reflection.
- **Configuration binding:** the source-generated binder (`EnableConfigurationBindingGenerator`) — no reflection.
- **Hosting:** `Host.CreateApplicationBuilder` for DI + logging, which is trim/AOT friendly.

`PublishAot` is enabled in the csproj, so the AOT/trim analyzers run on every build and
flag anything that would break a native publish.

## Run

```sh
dotnet run -- --name YourName
```

## Publish a native executable

```sh
dotnet publish -c Release -r linux-x64      # or win-x64, osx-arm64, ...
```

Native AOT needs a platform C toolchain — see
[the prerequisites](https://aka.ms/nativeaot-prerequisites) (on Ubuntu: `clang` + `zlib1g-dev`;
on Windows: the "Desktop development with C++" workload).

## Layout

- `Program.cs` — top-level entry point: builds the host, defines the CLI surface, dispatches.
- `Model/SampleOptions.cs` — strongly typed `appsettings.json` section.
- `Services/` — a sample DI service (`IGreeter` / `Greeter`).
- `appsettings.json` — configuration, loaded by the host builder.
